using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ColdHub.Console;
using ColdHub.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;

// Console：给人用的后台。人在这里登录，Console 再用自己持有的 Admin key 去调 Admin；Admin 只是个 API。
// 要点：浏览器 ↔ Console 用登录 Cookie；Console ↔ Admin 用 key；两套凭据互不通用。
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

builder.Services.AddOptions<ConsoleOptions>().BindConfiguration("Console").ValidateOnStart();
builder.Services.AddSingleton<IValidateOptions<ConsoleOptions>, ConsoleOptionsValidator>();

builder.Services.AddHttpClient("admin", (provider, client) =>
{
    ConsoleOptions options = provider.GetRequiredService<IOptions<ConsoleOptions>>().Value;
    client.BaseAddress = new Uri(options.AdminUrl.TrimEnd('/') + "/");
    client.Timeout = TimeSpan.FromSeconds(10);
    client.DefaultRequestHeaders.Authorization = new(ApiKeys.Scheme, options.AdminKey);
});
builder.Services.AddHttpClient("probe", client => client.Timeout = TimeSpan.FromSeconds(2));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    options.Cookie.Name = "coldhub.console";
    options.Cookie.HttpOnly = true;                       // 脚本读不到
    options.Cookie.SameSite = SameSiteMode.Strict;        // 别的站点发起的请求不带它：挡掉跨站请求伪造
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    // 接口不要跳转到登录页，直接回 401 / 403，由前端决定怎么显示
    options.Events.OnRedirectToLogin = context => Status(context.Response, StatusCodes.Status401Unauthorized);
    options.Events.OnRedirectToAccessDenied = context => Status(context.Response, StatusCodes.Status403Forbidden);
});
builder.Services.AddAuthorizationBuilder()
    .AddPolicy(Policies.Read, policy => policy.RequireAuthenticatedUser())
    .AddPolicy(Policies.Write, policy => policy.RequireRole(Roles.Admin));

WebApplication app = builder.Build();

// 页面只许加载自己域名下的脚本和样式（不允许内联），把 XSS 的口子关小
app.Use(async (context, next) =>
{
    context.Response.Headers.ContentSecurityPolicy = "default-src 'self'; frame-ancestors 'none'";
    context.Response.Headers.XContentTypeOptions = "nosniff";
    await next();
});
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapDefaultEndpoints();

// ---- 登录 ----
app.MapPost("/api/login", async (LoginRequest request, HttpContext context, IOptions<ConsoleOptions> options) =>
{
    ConsoleUser? user = options.Value.Users.Find(u => u.Name == request.Name);

    // 用户不存在和密码错用同一个回答，不让人探测账号；比较用固定时间算法
    if (user is null || !Same(user.Password, request.Password))
    {
        return Results.Unauthorized();
    }

    var identity = new ClaimsIdentity([new Claim(ClaimTypes.Name, user.Name), new Claim(ClaimTypes.Role, user.Role)],
        CookieAuthenticationDefaults.AuthenticationScheme);
    await context.SignInAsync(new ClaimsPrincipal(identity));
    return Results.Ok(new { user.Name, user.Role });
});

app.MapPost("/api/logout", async (HttpContext context) =>
{
    await context.SignOutAsync();
    return Results.NoContent();
}).RequireAuthorization(Policies.Read);

app.MapGet("/api/me", (ClaimsPrincipal user) => new { Name = user.Identity!.Name, Role = user.FindFirstValue(ClaimTypes.Role) })
    .RequireAuthorization(Policies.Read);

// ---- 转发给 Admin：每个接口点名一个策略，读的人人可用，写的只有 admin ----
app.MapGet("/api/sources", (HttpContext c, IHttpClientFactory f) => Forward(c, f, HttpMethod.Get, "sources"))
    .RequireAuthorization(Policies.Read);
app.MapPost("/api/sources", (HttpContext c, IHttpClientFactory f) => Forward(c, f, HttpMethod.Post, "sources", true))
    .RequireAuthorization(Policies.Write);
app.MapDelete("/api/sources/{name}", (string name, HttpContext c, IHttpClientFactory f) =>
    Forward(c, f, HttpMethod.Delete, $"sources/{Uri.EscapeDataString(name)}")).RequireAuthorization(Policies.Write);
app.MapPut("/api/sources/{name}/config", (string name, HttpContext c, IHttpClientFactory f) =>
    Forward(c, f, HttpMethod.Put, $"sources/{Uri.EscapeDataString(name)}/config", true)).RequireAuthorization(Policies.Write);

// 列出 key 也只给 admin：看谁拿着 key 本身就是敏感信息
app.MapGet("/api/keys", (HttpContext c, IHttpClientFactory f) => Forward(c, f, HttpMethod.Get, "keys"))
    .RequireAuthorization(Policies.Write);
app.MapPost("/api/keys", (HttpContext c, IHttpClientFactory f) => Forward(c, f, HttpMethod.Post, "keys", true))
    .RequireAuthorization(Policies.Write);
app.MapDelete("/api/keys/{id}", (string id, HttpContext c, IHttpClientFactory f) =>
    Forward(c, f, HttpMethod.Delete, $"keys/{Uri.EscapeDataString(id)}")).RequireAuthorization(Policies.Write);

// 各进程是否就绪：对每个配置的地址探测 /health/ready
app.MapGet("/api/health", async (IOptions<ConsoleOptions> options, IHttpClientFactory factory, CancellationToken ct) =>
{
    HttpClient probe = factory.CreateClient("probe");

    async Task<object> Check(string name, string url)
    {
        try
        {
            using HttpResponseMessage response = await probe.GetAsync($"{url.TrimEnd('/')}/health/ready", ct);
            return new { name, url, ready = response.IsSuccessStatusCode, detail = ((int)response.StatusCode).ToString() };
        }
        catch (Exception e) when (e is HttpRequestException or TaskCanceledException)
        {
            return new { name, url, ready = false, detail = "unreachable" };
        }
    }

    return await Task.WhenAll(options.Value.Services.Select(service => Check(service.Key, service.Value)));
}).RequireAuthorization(Policies.Read);

app.Run();

static bool Same(string expected, string given) =>
    CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(expected), Encoding.UTF8.GetBytes(given));

static Task Status(HttpResponse response, int code)
{
    response.StatusCode = code;
    return Task.CompletedTask;
}

// 把请求原样（状态码和内容）转给 Admin。这是一个白名单：只有上面列出的路径能到 Admin，其余的到不了。
static async Task<IResult> Forward(HttpContext context, IHttpClientFactory factory, HttpMethod method, string path,
    bool withBody = false)
{
    using var request = new HttpRequestMessage(method, path);

    if (withBody)
    {
        // 写请求必须是 application/json：浏览器里别的站点的表单提交不能发这种类型而不先经过预检
        if (!context.Request.HasJsonContentType())
        {
            return Results.StatusCode(StatusCodes.Status415UnsupportedMediaType);
        }

        request.Content = new StreamContent(context.Request.Body);
        request.Content.Headers.ContentType = new("application/json");
    }

    try
    {
        using HttpResponseMessage response = await factory.CreateClient("admin").SendAsync(request, context.RequestAborted);
        string body = await response.Content.ReadAsStringAsync(context.RequestAborted);
        return Results.Text(body, response.Content.Headers.ContentType?.ToString() ?? "text/plain", Encoding.UTF8,
            (int)response.StatusCode);
    }
    catch (Exception e) when (e is HttpRequestException or TaskCanceledException && !context.RequestAborted.IsCancellationRequested)
    {
        return Results.Problem("Admin is not reachable", statusCode: (int)HttpStatusCode.BadGateway);
    }
}

internal sealed record LoginRequest(string Name, string Password);

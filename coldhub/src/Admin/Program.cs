using ColdHub.Admin;
using ColdHub.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

// Admin：源的注册表、各源的配置、API key 的哈希。它只是个 API，人用的后台是 Console。
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

builder.Services.AddOptions<AdminOptions>().BindConfiguration("Admin").ValidateOnStart();
builder.Services.AddSingleton<IValidateOptions<AdminOptions>, AdminOptionsValidator>();
builder.Services.AddSingleton<Store>();

builder.Services.AddAuthentication(ApiKeys.Scheme).AddScheme<AuthenticationSchemeOptions, ApiKeyHandler>(ApiKeys.Scheme, null);
builder.Services.AddAuthorizationBuilder()
    .AddPolicy(Policies.Service, policy => policy.RequireRole(Roles.Service, Roles.Admin))
    .AddPolicy(Policies.Admin, policy => policy.RequireRole(Roles.Admin));

WebApplication app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.MapDefaultEndpoints();

// 每个端点都点名一个策略：没有"忘了加授权"的端点。
app.MapGet("/sources", (Store store) => store.Read(state => state.Sources.Select(s => new
{
    s.Name, s.Connector, s.Retired, s.Version, s.Config
}))).RequireAuthorization(Policies.Service);

app.MapPost("/sources", (NewSource request, Store store) =>
{
    if (!Channels.IsValidName(request.Name) || string.IsNullOrWhiteSpace(request.Connector))
    {
        return Results.BadRequest("name is 1-64 of A-Z a-z 0-9 . _ - ; connector is required");
    }

    return store.Write(state =>
    {
        // 名字永不复用：连已退役的也算占着
        if (state.Sources.Any(s => s.Name == request.Name))
        {
            return (Results.Conflict($"the name {request.Name} was registered before; names are never reused"), false);
        }

        state.Sources.Add(new SourceEntry { Name = request.Name, Connector = request.Connector });
        return (Results.Created($"/sources/{request.Name}", null), true);
    });
}).RequireAuthorization(Policies.Admin);

app.MapDelete("/sources/{name}", (string name, Store store) => store.Write(state =>
{
    SourceEntry? source = state.Sources.Find(s => s.Name == name);

    if (source is null)
    {
        return (Results.NotFound(), false);
    }

    source.Retired = true;
    source.Version++;
    return (Results.NoContent(), true);
})).RequireAuthorization(Policies.Admin);

// Source 轮询这个端点。ETag = 配置版本：没变就回 304，轮询几乎没有成本。
app.MapGet("/sources/{name}/config", (string name, HttpRequest request, HttpResponse response, Store store) =>
{
    SourceEntry? source = store.Read(state => state.Sources.Find(s => s.Name == name));

    if (source is null)
    {
        return Results.NotFound();
    }

    if (source.Retired)
    {
        return Results.StatusCode(StatusCodes.Status410Gone);
    }

    string etag = $"\"{source.Version}\"";
    response.Headers.ETag = etag;
    return request.Headers.IfNoneMatch == etag ? Results.StatusCode(StatusCodes.Status304NotModified) : Results.Ok(source.Config);
}).RequireAuthorization(Policies.Service);

app.MapPut("/sources/{name}/config", (string name, SourceConfig config, Store store) =>
{
    if (config.Devices.Count == 0 || config.Devices.Any(d => !Channels.IsValidName(d)) || config.IntervalMs < 50 ||
        config.MaxPerSecond is < 0 or > 1000)
    {
        return Results.BadRequest("devices: 1+ valid names; intervalMs >= 50; maxPerSecond 0-1000");
    }

    return store.Write(state =>
    {
        SourceEntry? source = state.Sources.Find(s => s.Name == name && !s.Retired);

        if (source is null)
        {
            return (Results.NotFound(), false);
        }

        source.Config = config;
        source.Version++;
        return (Results.Ok(new { source.Version }), true);
    });
}).RequireAuthorization(Policies.Admin);

// 列出 key：只给 id、归属人和状态，哈希也不外露
app.MapGet("/keys", (Store store) => store.Read(state => state.Keys.Select(k => new { k.Id, k.Owner, k.Revoked })))
    .RequireAuthorization(Policies.Admin);

// 创建 key：明文只在这一次响应里出现，库里只有哈希。
app.MapPost("/keys", (NewKey request, Store store) =>
{
    string key = ApiKeys.NewKey();
    string id = Guid.NewGuid().ToString("N")[..8];

    store.Write(state =>
    {
        state.Keys.Add(new KeyEntry { Id = id, Owner = request.Owner, Hash = ApiKeys.Hash(key) });
        return (0, true);
    });

    return Results.Ok(new { id, key });
}).RequireAuthorization(Policies.Admin);

app.MapDelete("/keys/{id}", (string id, Store store) => store.Write(state =>
{
    KeyEntry? entry = state.Keys.Find(k => k.Id == id);

    if (entry is null)
    {
        return (Results.NotFound(), false);
    }

    entry.Revoked = true;
    return (Results.NoContent(), true);
})).RequireAuthorization(Policies.Admin);

// Hub 轮询：有效 key 的哈希集合。
app.MapGet("/keys/hashes", (Store store) =>
    store.Read(state => state.Keys.Where(k => !k.Revoked).Select(k => k.Hash).ToArray()))
    .RequireAuthorization(Policies.Service);

app.Run();

internal sealed record NewSource(string Name, string Connector);

internal sealed record NewKey(string Owner);

public partial class Program;

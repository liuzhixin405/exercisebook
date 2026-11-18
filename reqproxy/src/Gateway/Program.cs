using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Transforms;

// The "forwarding service" (plays Console): people sign in here, with a cookie. A request for a region goes to that
// region's backend as the gateway itself: its key, and the person's email and role in two headers.

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<Dictionary<string, RegionOptions>>().BindConfiguration("Regions");
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "demo-session";
        // an API: 401 and 403, not a redirect to a login page
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    });
builder.Services.AddAuthorizationBuilder().AddPolicy("CanView", policy => policy.RequireAuthenticatedUser());

Dictionary<string, RegionOptions> regions = builder.Configuration.GetSection("Regions").Get<Dictionary<string, RegionOptions>>() ?? [];

builder.Services.AddSingleton<IProxyConfigProvider>(new RegionProxyConfig(regions));

// the Gateway's own endpoints are described as usual; the forwarded ones are YARP's routes, not endpoints, so they are
// added to the document by hand, one per region and entry of the list. Swagger UI is on this origin: once /login has
// answered, the browser sends the cookie with every "Try it out", as the web UI would.
builder.Services.AddOpenApi(options => options.AddDocumentTransformer((document, _, _) =>
{
    document.Info.Title = "Gateway (plays Console)";
    document.Info.Description = "1. POST /login (viewer@demo.local / viewer-pass, ops@demo.local / ops-pass, " +
                                "tech@demo.local / tech-pass). 2. Call /api/regions/{region}/...: forwarded to that " +
                                "region's backend. Try sending X-Demo-Role yourself: it never arrives.";
    document.Paths ??= new OpenApiPaths();

    foreach ((string code, RegionOptions region) in regions)
    {
        foreach ((string[] methods, string path) in RegionProxyConfig.Forwarded)
        {
            string shown = $"/api/regions/{code}{Regex.Replace(path, ":[a-z]+}", "}")}";
            var item = new OpenApiPathItem { Operations = new Dictionary<HttpMethod, OpenApiOperation>() };

            foreach (string method in methods)
            {
                var operation = new OpenApiOperation
                {
                    Summary = $"forwarded to {region.BackendUrl}/api{path} ({region.Name})",
                    Parameters = [],
                };

                foreach (Match name in Regex.Matches(path, "{([a-z]+)"))
                {
                    operation.Parameters.Add(new OpenApiParameter
                    {
                        Name = name.Groups[1].Value, In = ParameterLocation.Path, Required = true,
                        Schema = new OpenApiSchema { Type = JsonSchemaType.String }, Example = JsonValue.Create("Binance"),
                    });
                }

                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "X-Demo-Role", In = ParameterLocation.Header,
                    Description = "a forged role: the Gateway removes it", Schema = new OpenApiSchema { Type = JsonSchemaType.String },
                });

                if (method == "PUT")
                {
                    operation.RequestBody = new OpenApiRequestBody
                    {
                        Required = true,
                        Content = new Dictionary<string, OpenApiMediaType>
                        {
                            ["application/json"] = new() { Example = JsonNode.Parse("""{ "depth": 20, "expectedVersion": 1, "connector": null }""") },
                        },
                    };
                }

                item.Operations[new HttpMethod(method)] = operation;
            }

            document.Paths[shown] = item;
        }
    }

    return Task.CompletedTask;
}));
builder.Services.AddReverseProxy().AddTransforms(context =>
{
    RegionOptions region = regions[context.Route.ClusterId!];

    context.AddRequestTransform(transform =>
    {
        HttpRequestHeaders headers = transform.ProxyRequest.Headers;
        ClaimsPrincipal user = transform.HttpContext.User;

        // what the browser says about who it is is never passed on, and a forged X-Demo-* header goes with it
        foreach (string header in headers.Select(h => h.Key).Where(h => h.StartsWith("X-Demo-", StringComparison.OrdinalIgnoreCase)).ToList())
        {
            headers.Remove(header);
        }

        headers.Remove("Cookie");
        headers.Authorization = new AuthenticationHeaderValue("ApiKey", region.ApiKey);
        headers.Add("X-Demo-User", user.Identity?.Name);
        headers.Add("X-Demo-Role", user.FindFirstValue(ClaimTypes.Role));

        transform.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("Proxy").LogInformation(
            "{Method} {Incoming}  =>  {Destination}{Path}   as {Person} ({Role})",
            transform.HttpContext.Request.Method, transform.HttpContext.Request.Path, transform.DestinationPrefix,
            transform.Path, user.Identity?.Name, user.FindFirstValue(ClaimTypes.Role));
        return ValueTask.CompletedTask;
    });

    // nothing the backend answers can set a cookie here
    context.AddResponseTransform(transform =>
    {
        transform.HttpContext.Response.Headers.Remove("Set-Cookie");
        return ValueTask.CompletedTask;
    });
});

WebApplication app = builder.Build();

app.MapOpenApi();
app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "Gateway"));

app.UseAuthentication();
app.UseAuthorization();

// the people: a real one would be Entra ID or a table
Dictionary<string, (string Password, string Role)> people = new()
{
    ["viewer@demo.local"] = ("viewer-pass", "viewer"),
    ["ops@demo.local"] = ("ops-pass", "operation"),
    ["tech@demo.local"] = ("tech-pass", "technical"),
};

app.MapPost("/login", async (LoginRequest login, HttpContext context) =>
{
    if (!people.TryGetValue(login.Email, out var person) || person.Password != login.Password)
    {
        return Results.Unauthorized();
    }

    var identity = new ClaimsIdentity([new Claim(ClaimTypes.Name, login.Email), new Claim(ClaimTypes.Role, person.Role)],
        CookieAuthenticationDefaults.AuthenticationScheme);
    await context.SignInAsync(new ClaimsPrincipal(identity));
    return Results.Ok(new { email = login.Email, role = person.Role });
});

app.MapPost("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync();
    return Results.NoContent();
});

app.MapGet("/api/me", (ClaimsPrincipal user) => new { email = user.Identity?.Name, role = user.FindFirstValue(ClaimTypes.Role) })
    .RequireAuthorization("CanView");

app.MapGet("/api/regions", () => regions.Select(r => new { code = r.Key, name = r.Value.Name }))
    .RequireAuthorization("CanView");

app.MapReverseProxy();

app.Run();

internal sealed record LoginRequest(string Email, string Password);

internal sealed class RegionOptions
{
    public string Name { get; init; } = "";

    public string BackendUrl { get; init; } = "";

    public string ApiKey { get; init; } = "";
}

/// <summary>A route for each of a region's endpoints on the list, and a cluster for its backend.</summary>
internal sealed class RegionProxyConfig(Dictionary<string, RegionOptions> regions) : IProxyConfigProvider
{
    // the backend's endpoints a page may call; nothing else is forwarded (a call to another one is a 404 here)
    internal static readonly (string[] Methods, string Path)[] Forwarded =
    [
        (["GET"], "/whoami"),
        (["GET"], "/sources"),
        (["GET", "PUT"], "/sources/{name}"),
    ];

    private readonly Lazy<IProxyConfig> _config = new(() => new Config(regions));

    public IProxyConfig GetConfig() => _config.Value;

    private sealed class Config(Dictionary<string, RegionOptions> regions) : IProxyConfig
    {
        public IReadOnlyList<RouteConfig> Routes { get; } =
        [
            .. regions.SelectMany(region => Forwarded.Select((endpoint, index) => new RouteConfig
            {
                RouteId = $"{region.Key}-{index}",
                ClusterId = region.Key,
                AuthorizationPolicy = "CanView",
                Match = new RouteMatch { Methods = endpoint.Methods, Path = $"/api/regions/{region.Key}{endpoint.Path}" },
                Transforms =
                [
                    new Dictionary<string, string> { ["PathRemovePrefix"] = $"/api/regions/{region.Key}" },
                    new Dictionary<string, string> { ["PathPrefix"] = "/api" },
                ],
            })),
        ];

        public IReadOnlyList<ClusterConfig> Clusters { get; } =
        [
            .. regions.Select(region => new ClusterConfig
            {
                ClusterId = region.Key,
                Destinations = new Dictionary<string, DestinationConfig> { ["backend"] = new() { Address = region.Value.BackendUrl } },
            }),
        ];

        public IChangeToken ChangeToken { get; } = new CancellationChangeToken(CancellationToken.None);
    }
}

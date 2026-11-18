using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;

// The "actual service" (plays Admin): an API only. It has no login and never reads a cookie. A request is accepted
// only with its region's key; the key being the gateway's, the person's email and role come in two headers.

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<BackendOptions>().BindConfiguration("Backend");
builder.Services.AddOpenApi(options => options.AddDocumentTransformer((document, _, _) =>
{
    // what a caller must send, entered once under Authorize: the region's key and the person
    document.Info.Title = "Backend (plays Admin)";
    document.Info.Description = "Called by the Gateway. To call it straight from here, Authorize with `ApiKey <the region's key>` " +
                                "(hk: `ApiKey hk-key-0123456789`), a person and a role: what the Gateway would send.";
    document.Components ??= new OpenApiComponents();
    document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
    document.Components.SecuritySchemes["ApiKey"] = new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey, In = ParameterLocation.Header, Name = "Authorization",
        Description = "`ApiKey hk-key-0123456789` (hk) or `ApiKey ny-key-9876543210` (ny)",
    };
    document.Components.SecuritySchemes["User"] = new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey, In = ParameterLocation.Header, Name = "X-Demo-User", Description = "an email",
    };
    document.Components.SecuritySchemes["Role"] = new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey, In = ParameterLocation.Header, Name = "X-Demo-Role",
        Description = "viewer, operation or technical",
    };
    document.Security =
    [
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("ApiKey", document)] = [],
            [new OpenApiSecuritySchemeReference("User", document)] = [],
            [new OpenApiSecuritySchemeReference("Role", document)] = [],
        },
    ];
    return Task.CompletedTask;
}));
builder.Services.AddAuthentication(KeyHandler.SchemeName).AddScheme<AuthenticationSchemeOptions, KeyHandler>(KeyHandler.SchemeName, null);
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("CanView", policy => policy.RequireAuthenticatedUser())
    .AddPolicy("CanEdit", policy => policy.RequireRole("operation", "technical"));

// in memory, so that a demo starts the same every time
var sources = new Dictionary<string, SourceState>
{
    ["Binance"] = new("binance-spot", 10, 1),
    ["Kraken"] = new("kraken", 5, 1),
};

WebApplication app = builder.Build();
string region = app.Services.GetRequiredService<IOptions<BackendOptions>>().Value.Region;

app.MapOpenApi();
app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", $"Backend {region}"));

app.UseAuthentication();
app.UseAuthorization();

// what the backend received: the proof of what the gateway did to the request
app.MapGet("/api/whoami", (HttpContext context, ClaimsPrincipal user) => new
{
    region,
    person = user.Identity?.Name,
    role = user.FindFirstValue(ClaimTypes.Role),
    cookieReceived = context.Request.Headers.ContainsKey("Cookie"),
    authorization = context.Request.Headers.Authorization.ToString().Split(' ')[0] + " ***",
    demoHeaders = context.Request.Headers.Where(h => h.Key.StartsWith("X-Demo-", StringComparison.OrdinalIgnoreCase))
        .ToDictionary(h => h.Key, h => h.Value.ToString()),
}).RequireAuthorization("CanView");

app.MapGet("/api/sources", () => sources.Select(s => new { name = s.Key, s.Value.Connector, s.Value.Depth, s.Value.Version }))
    .RequireAuthorization("CanView");

app.MapGet("/api/sources/{name}", (string name) =>
        sources.TryGetValue(name, out SourceState? s) ? Results.Ok(new { name, s.Connector, s.Depth, s.Version }) : Results.NotFound())
    .RequireAuthorization("CanView");

app.MapPut("/api/sources/{name}", (string name, EditRequest edit, ClaimsPrincipal user) =>
{
    if (!sources.TryGetValue(name, out SourceState? current))
    {
        return Results.NotFound();
    }

    // like Admin: whatever the UI showed, changing the connector is a technical person's, checked here
    if (edit.Connector is not null && edit.Connector != current.Connector && !user.IsInRole("technical"))
    {
        return Results.Problem("Only a technical person changes the connector", statusCode: 403);
    }

    if (edit.ExpectedVersion != current.Version)
    {
        return Results.Conflict(new { name, current.Connector, current.Depth, current.Version });
    }

    var saved = new SourceState(edit.Connector ?? current.Connector, edit.Depth, current.Version + 1);
    sources[name] = saved;
    app.Logger.LogInformation("{Region}: {Person} ({Role}) saved {Source}: depth {Depth}, connector {Connector}, version {Version}",
        region, user.Identity?.Name, user.FindFirstValue(ClaimTypes.Role), name, saved.Depth, saved.Connector, saved.Version);
    return Results.Ok(new { name, saved.Connector, saved.Depth, saved.Version });
}).RequireAuthorization("CanEdit");

app.Run();

internal sealed record SourceState(string Connector, int Depth, long Version);

internal sealed record EditRequest(int Depth, long ExpectedVersion, string? Connector);

internal sealed class BackendOptions
{
    public string Region { get; init; } = "";

    public string ApiKey { get; init; } = "";
}

internal sealed class KeyHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IOptions<BackendOptions> backend) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "ApiKey";

    private static readonly string[] Roles = ["viewer", "operation", "technical"];

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string header = Request.Headers.Authorization.ToString();

        if (!header.StartsWith(SchemeName + " ", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (header[(SchemeName.Length + 1)..].Trim() != backend.Value.ApiKey)
        {
            return Task.FromResult(AuthenticateResult.Fail("not this region's key"));
        }

        string person = Request.Headers["X-Demo-User"].ToString().Trim();
        string role = Request.Headers["X-Demo-Role"].ToString();

        if (person.Length is 0 or > 256 || !Roles.Contains(role))
        {
            return Task.FromResult(AuthenticateResult.Fail("the gateway's key needs X-Demo-User and a X-Demo-Role"));
        }

        var identity = new ClaimsIdentity([new Claim(ClaimTypes.Name, person), new Claim(ClaimTypes.Role, role)], SchemeName);
        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity), SchemeName)));
    }
}

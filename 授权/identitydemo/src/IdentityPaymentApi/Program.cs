using System.Diagnostics;
using FluentValidation.AspNetCore;
using IdentityPaymentApi.Application;
using IdentityPaymentApi.Application.Behaviours;
using IdentityPaymentApi.Authorization;
using IdentityPaymentApi.Domain;
using IdentityPaymentApi.Infrastructure;
using IdentityPaymentApi.Infrastructure.Extensions;
using IdentityPaymentApi.Infrastructure.Middlewares;
using IdentityPaymentApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationLayer();
builder.Services.AddInfrastructureServices(builder);
builder.Services.AddBehavior(typeof(ValidationBehavior<,>));

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("payments.create", policy => policy.Requirements.Add(new PermissionRequirement("payments.create")));
    options.AddPolicy("payments.read", policy => policy.Requirements.Add(new PermissionRequirement("payments.read")));
    options.AddPolicy("payments.view", policy => policy.Requirements.Add(new PermissionRequirement("payments.view")));
    options.AddPolicy("payments.delete", policy => policy.Requirements.Add(new PermissionRequirement("payments.delete")));
    options.AddPolicy("payments.manage", policy => policy.Requirements.Add(new PermissionRequirement("payments.manage")));
});

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCustomSwagger();
builder.Services.AddAnyCors();
builder.Services.AddHealthChecks();
builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

await EnsureDatabaseSeededAsync(app);

if (app.Environment.IsDevelopment())
{
    app.UseCustomSwagger();

    // Open Swagger UI in the default browser when the application has started.
    app.Lifetime.ApplicationStarted.Register(() =>
    {
        try
        {
            // Prefer the first configured URL, fall back to common HTTPS localhost
            var baseUrl = app.Urls.FirstOrDefault() ?? builder.Configuration["urls"] ?? "https://localhost:5001";
            var swaggerUrl = baseUrl.TrimEnd('/') + "/swagger";
            var psi = new ProcessStartInfo { FileName = swaggerUrl, UseShellExecute = true };
            Process.Start(psi);
        }
        catch
        {
            // ignore - opening browser is a convenience only
        }
    });
}

app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseHttpsRedirection();
app.UseAnyCors();

app.UseAuthentication();
app.UseAuthorization();
app.UseHealthChecks("/health");

app.MapControllers();
app.UseSerilogRequestLogging();

await app.RunAsync();

static async Task EnsureDatabaseSeededAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var identityContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var paymentsContext = scope.ServiceProvider.GetRequiredService<IdentityPaymentApi.Domain.PaymentsDbContext>();

    try
    {
        var identityMigrations = identityContext.Database.GetMigrations();
        if (identityMigrations != null && identityMigrations.Any())
        {
            await identityContext.Database.MigrateAsync();
        }
        else
        {
            await identityContext.Database.EnsureCreatedAsync();
        }

        var paymentsMigrations = paymentsContext.Database.GetMigrations();
        if (paymentsMigrations != null && paymentsMigrations.Any())
        {
            await paymentsContext.Database.MigrateAsync();
        }
        else
        {
            await paymentsContext.Database.EnsureCreatedAsync();
        }
    }
    catch
    {
        try { await identityContext.Database.EnsureCreatedAsync(); } catch { }
        try { await paymentsContext.Database.EnsureCreatedAsync(); } catch { }
    }

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    if (!await roleManager.RoleExistsAsync("Payer"))
    {
        await roleManager.CreateAsync(new IdentityRole("Payer"));
    }
}

using System.Text;
using IdentityPaymentApi.Models;
using IdentityPaymentApi.Services;
using IdentityPaymentApi.Application.Services;
using IdentityPaymentApi.Infrastructure.Services;
using IdentityPaymentApi.Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using Microsoft.OpenApi.Models;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<JwtSettings>(jwtSection);
var jwtSettings = jwtSection.Get<JwtSettings>() ?? new JwtSettings();

// Register Identity DbContext (users/roles) and Payments DbContext (business data)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("IdentityConnection")));
builder.Services.AddDbContext<PaymentsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PaymentsConnection")));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var key = Encoding.UTF8.GetBytes(jwtSettings.Secret);
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Payments", policy => policy.RequireRole("Payer"));
});

builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "IdentityPaymentApi", Version = "v1" });
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGci...\"",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});

var app = builder.Build();

await EnsureDatabaseSeededAsync(app);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "IdentityPaymentApi v1"));

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

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();

static async Task EnsureDatabaseSeededAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var identityContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var paymentsContext = scope.ServiceProvider.GetRequiredService<IdentityPaymentApi.Domain.PaymentsDbContext>();
    // If there are migrations available, apply them. Otherwise create the schema
    // directly from the model. MigrateAsync does nothing when no migrations exist,
    // so EnsureCreatedAsync is required to create tables in that case.
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
        // As a last resort, try EnsureCreated to ensure the schema exists for seeding.
        try { await identityContext.Database.EnsureCreatedAsync(); } catch { }
        try { await paymentsContext.Database.EnsureCreatedAsync(); } catch { }
    }
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    if (!await roleManager.RoleExistsAsync("Payer"))
    {
        await roleManager.CreateAsync(new IdentityRole("Payer"));
    }
}

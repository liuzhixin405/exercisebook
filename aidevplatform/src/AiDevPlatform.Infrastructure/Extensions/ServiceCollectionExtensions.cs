using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AiDevPlatform.Infrastructure.Data;
using AiDevPlatform.Infrastructure.Repositories;
using AiDevPlatform.Infrastructure.Services;
using AiDevPlatform.Core.Interfaces;
using AiDevPlatform.Shared.Models;

namespace AiDevPlatform.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add Entity Framework with In-Memory Database
        services.AddDbContext<AiDevPlatformDbContext>(options =>
        {
            options.UseInMemoryDatabase("LowCodePlatformDb");
        });

        services.AddIdentityCore<User>() // [联网-官方: https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.identityservicecollectionextensions.addidentitycore?view=aspnetcore-8.0]
            .AddRoles<IdentityRole<Guid>>() // [联网-官方: https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.identityservicecollectionextensions.addidentitycore?view=aspnetcore-8.0]
            .AddEntityFrameworkStores<AiDevPlatformDbContext>(); // [联网-官方: https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.identityentityframeworkbuilderextensions.addentityframeworkstores?view=aspnetcore-8.0]

        // Add repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IApplicationRepository, ApplicationRepository>();
        services.AddScoped<IApplicationPageRepository, ApplicationPageRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IComponentRepository, ComponentRepository>();
        services.AddScoped<IDataModelRepository, DataModelRepository>();
        services.AddScoped<IWorkflowRepository, WorkflowRepository>();

        // Add services
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<ICacheService, CacheService>();
        services.AddScoped<ICodeGeneratorService, CodeGeneratorService>();
        services.AddScoped<IWorkflowEngineService, WorkflowEngineService>();

        // [来源: aspnet-core-aspnetcore-10.0#0247 p1642]
        services.AddHttpClient("LocalAI", client =>
        {
            client.Timeout = TimeSpan.FromSeconds(90);
            client.MaxResponseContentBufferSize = 262144;
        }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false, UseProxy = false });
        services.AddScoped<IPageDraftGenerator, LocalAiPageDraftGenerator>();
        services.AddHttpClient("LocalAI-Code", client =>
        {
            client.Timeout = TimeSpan.FromSeconds(180);
            client.MaxResponseContentBufferSize = 524288;
        }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false, UseProxy = false });
        services.AddScoped<IAiDevelopmentGenerator, LocalAiDevelopmentGenerator>();
        services.AddScoped<IDevelopmentStore, JsonDevelopmentStore>();
        services.AddHttpClient("Development-Verification", client =>
        {
            client.Timeout = TimeSpan.FromSeconds(90);
            client.MaxResponseContentBufferSize = 65536;
        }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false, UseProxy = false });
        services.AddScoped<IDevelopmentVerifier, HttpDevelopmentVerifier>();

        // Add memory cache
        services.AddMemoryCache();

        return services;
    }
}
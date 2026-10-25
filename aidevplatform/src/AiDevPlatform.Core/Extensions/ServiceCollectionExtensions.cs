using Microsoft.Extensions.DependencyInjection;
using AiDevPlatform.Core.Features.Applications;

namespace AiDevPlatform.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        // Add MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly));

        services.AddScoped<ApplicationManagementService>();
        services.AddScoped<DevelopmentWorkflow>();

        return services;
    }
}

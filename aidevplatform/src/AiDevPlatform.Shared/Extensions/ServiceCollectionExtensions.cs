using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace AiDevPlatform.Shared.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLowCodePlatformServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add common services here
        return services;
    }
}

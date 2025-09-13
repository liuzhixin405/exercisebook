using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Bus
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCommandBus(this IServiceCollection services)
        {
            services.AddScoped<ICommandBus, CommandBus>();
            return services;
        }

        public static IServiceCollection AddCommandBehaviorOpenGeneric<TBehavior>(this IServiceCollection services)
            where TBehavior :class
        {
            services.AddScoped(typeof(ICommandPipelineBehavior<,>), typeof(TBehavior));
            return services;
        }
    }
}

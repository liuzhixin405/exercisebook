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
            // 使用单例模式以提高性能，因为CommandBus本身是无状态的（除了缓存）
            services.AddSingleton<ICommandBus, CommandBus>();
            return services;
        }

        public static IServiceCollection AddCommandBehaviorOpenGeneric<TBehavior>(this IServiceCollection services)
            where TBehavior :class
        {
            services.AddScoped(typeof(ICommandPipelineBehavior<,>), typeof(TBehavior));
            return services;
        }

        // 添加性能监控扩展方法
        public static IServiceCollection AddCommandBusWithMetrics(this IServiceCollection services)
        {
            services.AddSingleton<ICommandBus, CommandBus>();
            // 可以在这里添加性能监控相关的服务
            return services;
        }
    }
}

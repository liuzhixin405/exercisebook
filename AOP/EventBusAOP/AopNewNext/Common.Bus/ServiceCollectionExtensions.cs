using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Bus
{
    public static class ServiceCollectionExtensions
    {

        /// <summary>
        /// 添加基于委托的泛型优化CommandBus实现（推荐）
        /// 通过委托简化泛型打包，保持类型安全
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="maxConcurrency">最大并发数，默认为处理器核心数的2倍</param>
        public static IServiceCollection AddDelegateBasedCommandBus(this IServiceCollection services,
            int? maxConcurrency = null)
        {
            services.AddSingleton<ICommandBus>(provider =>
            {
                var logger = provider.GetService<ILogger<DelegateBasedCommandBus>>();
                var concurrency = maxConcurrency ?? Environment.ProcessorCount * 2;
                return new DelegateBasedCommandBus(provider, logger, concurrency);
            });
            return services;
        }



        public static IServiceCollection AddCommandBehaviorOpenGeneric<TBehavior>(this IServiceCollection services)
            where TBehavior :class
        {
            services.AddScoped(typeof(ICommandPipelineBehavior<,>), typeof(TBehavior));
            return services;
        }

        /// <summary>
        /// 自动注册所有CommandHandler和CommandProcessor
        /// </summary>
        public static IServiceCollection AddCommandHandlers(this IServiceCollection services)
        {
            // 注册所有ICommandHandler实现
            services.Scan(scan => scan
                .FromCallingAssembly()
                .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());


            return services;
        }

    }

}

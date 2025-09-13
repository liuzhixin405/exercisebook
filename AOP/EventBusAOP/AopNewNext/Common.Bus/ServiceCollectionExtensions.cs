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
        /// 添加基于时间戳的优化CommandBus实现（推荐）
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="maxConcurrency">最大并发数，默认为处理器核心数的2倍</param>
        /// <param name="enableBatchProcessing">是否启用批处理，默认为true</param>
        /// <param name="batchWindowSize">批处理时间窗口大小，默认为50毫秒</param>
        public static IServiceCollection AddTimeBasedCommandBus(this IServiceCollection services,
            int? maxConcurrency = null, bool enableBatchProcessing = true, TimeSpan? batchWindowSize = null)
        {
            services.AddSingleton<ICommandBus>(provider =>
            {
                var logger = provider.GetService<ILogger<TimeBasedCommandBus>>();
                var concurrency = maxConcurrency ?? Environment.ProcessorCount * 2;
                var windowSize = batchWindowSize ?? TimeSpan.FromMilliseconds(50);
                return new TimeBasedCommandBus(provider, logger, concurrency, enableBatchProcessing, windowSize);
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

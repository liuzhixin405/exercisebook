using ECommerce.Core.EventBus;
using ECommerce.Domain.Interfaces;
using ECommerce.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ECommerce.Infrastructure.EventBus
{
    /// <summary>
    /// 事件总线服务集合扩展
    /// </summary>
    public static class EventBusServiceCollectionExtensions
    {
        /// <summary>
        /// 添加RabbitMQ事件总线服务
        /// </summary>
        public static IServiceCollection AddRabbitMQEventBus(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // 配置RabbitMQ设置
            var rabbitMQSettings = new RabbitMQSettings
            {
                HostName = configuration.GetSection("RabbitMQ:HostName").Value ?? "localhost",
                Port = int.TryParse(configuration.GetSection("RabbitMQ:Port").Value, out var port) ? port : 5672,
                UserName = configuration.GetSection("RabbitMQ:UserName").Value ?? "guest",
                Password = configuration.GetSection("RabbitMQ:Password").Value ?? "guest",
                VirtualHost = configuration.GetSection("RabbitMQ:VirtualHost").Value ?? "/",
                BrokerName = configuration.GetSection("RabbitMQ:BrokerName").Value ?? "ecommerce_event_bus",
                QueueName = configuration.GetSection("RabbitMQ:QueueName").Value ?? "ecommerce_event_queue"
            };
            services.AddSingleton(rabbitMQSettings);

            // 注册事件总线 - 同时注册接口和具体实现
            services.AddSingleton<RabbitMQEventBus>();
            services.AddSingleton<IEventBus>(provider => provider.GetRequiredService<RabbitMQEventBus>());

            // 注册事件订阅管理器
            services.AddSingleton<EventSubscriptionManager>();

            // 注册所有事件处理器
            RegisterEventHandlers(services);

            return services;
        }

        /// <summary>
        /// 注册所有事件处理器
        /// </summary>
        private static void RegisterEventHandlers(IServiceCollection services)
        {
            // 事件处理器将在Application项目中注册
            // 这里只注册基础设施服务
        }
    }
}

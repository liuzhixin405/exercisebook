using ECommerce.Core.EventBus;
using ECommerce.Domain.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ECommerce.Infrastructure.EventBus
{
    /// <summary>
    /// 事件总线启动服务
    /// </summary>
    public class EventBusStartupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EventBusStartupService> _logger;
        private readonly EventSubscriptionManager _subscriptionManager;
        private readonly RabbitMQEventBus _eventBus;

        public EventBusStartupService(
            IServiceProvider serviceProvider,
            EventSubscriptionManager subscriptionManager,
            RabbitMQEventBus eventBus,
            ILogger<EventBusStartupService> logger)
        {
            _serviceProvider = serviceProvider;
            _subscriptionManager = subscriptionManager;
            _eventBus = eventBus;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Starting event bus configuration...");

                // 配置事件订阅
                await ConfigureEventSubscriptionsAsync();

                // 启动事件消费
                _eventBus.StartConsuming();

                _logger.LogInformation("Event bus configuration completed successfully");

                // 等待取消信号
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during event bus startup");
                throw;
            }
        }

        /// <summary>
        /// 配置事件订阅
        /// </summary>
        private async Task ConfigureEventSubscriptionsAsync()
        {
            try
            {
                // 使用反射动态发现和注册事件处理器
                await DiscoverAndRegisterEventHandlersAsync();

                _logger.LogInformation("Event subscriptions configured successfully");
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to configure event subscriptions");
                throw;
            }
        }

        /// <summary>
        /// 动态发现和注册事件处理器
        /// </summary>
        private async Task DiscoverAndRegisterEventHandlersAsync()
        {
            try
            {
                // 获取所有已注册的事件处理器类型
                var handlerTypes = _serviceProvider.GetServices<object>()
                    .Where(s => s.GetType().GetInterfaces()
                        .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<>)))
                    .Select(s => s.GetType())
                    .ToList();

                foreach (var handlerType in handlerTypes)
                {
                    var eventHandlerInterface = handlerType.GetInterfaces()
                        .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<>));
                    
                    if (eventHandlerInterface != null)
                    {
                        var eventType = eventHandlerInterface.GetGenericArguments()[0];
                        var method = typeof(EventSubscriptionManager).GetMethod("AddSubscription");
                        var genericMethod = method?.MakeGenericMethod(eventType, handlerType);
                        
                        if (genericMethod != null)
                        {
                            genericMethod.Invoke(_subscriptionManager, null);
                            _logger.LogInformation("Discovered and registered event handler: {HandlerType} for event {EventType}", 
                                handlerType.Name, eventType.Name);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to discover and register event handlers");
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping event bus startup service...");
            await base.StopAsync(cancellationToken);
        }
    }
}

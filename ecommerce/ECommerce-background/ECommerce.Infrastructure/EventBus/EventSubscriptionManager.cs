using ECommerce.Core.EventBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ECommerce.Infrastructure.EventBus
{
    /// <summary>
    /// 事件订阅管理器
    /// </summary>
    public class EventSubscriptionManager
    {
        private readonly IEventBus _eventBus;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<EventSubscriptionManager> _logger;
        private readonly List<EventSubscription> _subscriptions;

        public EventSubscriptionManager(
            IEventBus eventBus,
            IServiceProvider serviceProvider,
            ILogger<EventSubscriptionManager> logger)
        {
            _eventBus = eventBus;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _subscriptions = new List<EventSubscription>();
        }

        /// <summary>
        /// 添加事件订阅
        /// </summary>
        public void AddSubscription<T, TH>()
            where T : class
            where TH : IEventHandler<T>
        {
            var eventName = typeof(T).Name;
            var handlerName = typeof(TH).Name;

            // 检查是否已存在订阅
            if (HasSubscription<T, TH>())
            {
                _logger.LogWarning("Subscription already exists for event {EventName} with handler {HandlerName}", eventName, handlerName);
                return;
            }

            // 注册订阅
            _eventBus.Subscribe<T, TH>();

            // 记录订阅信息
            var subscription = new EventSubscription
            {
                EventType = typeof(T),
                HandlerType = typeof(TH),
                EventName = eventName,
                HandlerName = handlerName
            };

            _subscriptions.Add(subscription);
            _logger.LogInformation("Added subscription for event {EventName} with handler {HandlerName}", eventName, handlerName);
        }

        /// <summary>
        /// 移除事件订阅
        /// </summary>
        public void RemoveSubscription<T, TH>()
            where T : class
            where TH : IEventHandler<T>
        {
            var eventName = typeof(T).Name;
            var handlerName = typeof(TH).Name;

            if (!HasSubscription<T, TH>())
            {
                _logger.LogWarning("Subscription does not exist for event {EventName} with handler {HandlerName}", eventName, handlerName);
                return;
            }

            // 取消订阅
            _eventBus.Unsubscribe<T, TH>();

            // 移除订阅记录
            var subscription = _subscriptions.FirstOrDefault(s => 
                s.EventType == typeof(T) && s.HandlerType == typeof(TH));

            if (subscription != null)
            {
                _subscriptions.Remove(subscription);
                _logger.LogInformation("Removed subscription for event {EventName} with handler {HandlerName}", eventName, handlerName);
            }
        }

        /// <summary>
        /// 检查是否存在订阅
        /// </summary>
        public bool HasSubscription<T, TH>()
            where T : class
            where TH : IEventHandler<T>
        {
            return _subscriptions.Any(s => 
                s.EventType == typeof(T) && s.HandlerType == typeof(TH));
        }

        /// <summary>
        /// 获取所有订阅
        /// </summary>
        public IEnumerable<EventSubscription> GetSubscriptions()
        {
            return _subscriptions.AsReadOnly();
        }

        /// <summary>
        /// 获取指定事件的订阅
        /// </summary>
        public IEnumerable<EventSubscription> GetSubscriptionsForEvent<T>() where T : class
        {
            var eventType = typeof(T);
            return _subscriptions.Where(s => s.EventType == eventType);
        }

        /// <summary>
        /// 清空所有订阅
        /// </summary>
        public void Clear()
        {
            _subscriptions.Clear();
            _logger.LogInformation("All event subscriptions cleared");
        }
    }
}

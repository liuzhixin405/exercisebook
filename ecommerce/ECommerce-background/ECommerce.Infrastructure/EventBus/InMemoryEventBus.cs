using ECommerce.Core.EventBus;
using Microsoft.Extensions.Logging;

namespace ECommerce.Infrastructure.EventBus
{
    public class InMemoryEventBus : IEventBus
    {
        private readonly Dictionary<Type, List<Type>> _handlers = new();
        private readonly ILogger<InMemoryEventBus> _logger;

        public InMemoryEventBus(ILogger<InMemoryEventBus> logger)
        {
            _logger = logger;
        }

        public async Task PublishAsync<T>(T @event) where T : class
        {
            var eventType = typeof(T);
            
            if (_handlers.ContainsKey(eventType))
            {
                var handlerTypes = _handlers[eventType];
                var tasks = new List<Task>();

                foreach (var handlerType in handlerTypes)
                {
                    try
                    {
                        // 这里需要从DI容器获取处理器实例
                        // 在实际使用中，应该通过IServiceProvider来解析
                        _logger.LogInformation("Would process event {EventType} with handler {HandlerType}", 
                            eventType.Name, handlerType.Name);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing event {EventType} with handler {HandlerType}", 
                            eventType.Name, handlerType.Name);
                    }
                }

                await Task.WhenAll(tasks);
                _logger.LogInformation("Published event {EventType} to {HandlerCount} handlers", 
                    eventType.Name, handlerTypes.Count);
            }
            else
            {
                _logger.LogWarning("No handlers found for event type {EventType}", eventType.Name);
            }
        }

        public void Subscribe<T, TH>() where T : class where TH : IEventHandler<T>
        {
            var eventType = typeof(T);
            var handlerType = typeof(TH);
            
            if (!_handlers.ContainsKey(eventType))
            {
                _handlers[eventType] = new List<Type>();
            }
            
            if (!_handlers[eventType].Contains(handlerType))
            {
                _handlers[eventType].Add(handlerType);
                _logger.LogInformation("Subscribed handler {HandlerType} to event {EventType}", 
                    handlerType.Name, eventType.Name);
            }
        }

        public void Unsubscribe<T, TH>() where T : class where TH : IEventHandler<T>
        {
            var eventType = typeof(T);
            var handlerType = typeof(TH);
            
            if (_handlers.ContainsKey(eventType))
            {
                _handlers[eventType].Remove(handlerType);
                _logger.LogInformation("Unsubscribed handler {HandlerType} from event {EventType}", 
                    handlerType.Name, eventType.Name);
            }
        }
    }
}

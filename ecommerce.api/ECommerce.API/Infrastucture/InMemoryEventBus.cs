using ECommerce.API.Application;
using System.Collections.Concurrent;

namespace ECommerce.API.Infrastucture
{
    // 内存事件总线实现
    public class InMemoryEventBus : IEventBus
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ConcurrentDictionary<Type, List<Type>> _handlers = new();

        public InMemoryEventBus(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task PublishAsync<T>(T @event) where T : IDomainEvent
        {
            if (_handlers.TryGetValue(typeof(T), out var handlerTypes))
            {
                foreach (var handlerType in handlerTypes)
                {
                    var handler = (IEventHandler<T>)_serviceProvider.GetRequiredService(handlerType);
                    await handler.HandleAsync(@event);
                }
            }
        }

        public void Subscribe<T, H>() where T : IDomainEvent where H : IEventHandler<T>
        {
            _handlers.AddOrUpdate(typeof(T),
                new List<Type> { typeof(H) },
                (key, existing) => { existing.Add(typeof(H)); return existing; });
        }
    }
}

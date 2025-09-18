using Framework.Core.Abstractions.Events;
using System.Collections.Concurrent;

namespace Framework.Infrastructure.Events;

/// <summary>
/// 事件总线实现 - 观察者模式
/// 提供事件发布和订阅的实现
/// </summary>
public class EventBus : IEventBus
{
    private readonly ConcurrentDictionary<Type, List<EventSubscription>> _subscriptions;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="serviceProvider">服务提供者</param>
    public EventBus(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _subscriptions = new ConcurrentDictionary<Type, List<EventSubscription>>();
    }

    /// <inheritdoc />
    public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : class
    {
        if (@event == null)
            throw new ArgumentNullException(nameof(@event));

        var eventType = typeof(TEvent);
        if (!_subscriptions.TryGetValue(eventType, out var subscriptions))
        {
            return;
        }

        // 按优先级排序处理器
        var sortedSubscriptions = subscriptions.OrderBy(s => s.Priority).ToList();

        foreach (var subscription in sortedSubscriptions)
        {
            if (subscription.ShouldHandle(@event))
            {
                await subscription.HandleAsync(@event);
            }
        }
    }

    /// <inheritdoc />
    public void Publish<TEvent>(TEvent @event) where TEvent : class
    {
        PublishAsync(@event).GetAwaiter().GetResult();
    }

    /// <inheritdoc />
    public string Subscribe<TEvent>(IEventHandler<TEvent> handler) where TEvent : class
    {
        if (handler == null)
            throw new ArgumentNullException(nameof(handler));

        var subscription = new EventSubscription<TEvent>(handler);
        var eventType = typeof(TEvent);

        _subscriptions.AddOrUpdate(
            eventType,
            new List<EventSubscription> { subscription },
            (key, existing) =>
            {
                existing.Add(subscription);
                return existing;
            });

        return subscription.Id;
    }

    /// <inheritdoc />
    public string Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : class
    {
        if (handler == null)
            throw new ArgumentNullException(nameof(handler));

        var eventHandler = new DelegateEventHandler<TEvent>(handler);
        return Subscribe(eventHandler);
    }

    /// <inheritdoc />
    public void Unsubscribe(string subscriptionId)
    {
        if (string.IsNullOrEmpty(subscriptionId))
            return;

        foreach (var kvp in _subscriptions.ToList())
        {
            var subscriptions = kvp.Value;
            var subscriptionToRemove = subscriptions.FirstOrDefault(s => s.Id == subscriptionId);
            if (subscriptionToRemove != null)
            {
                subscriptions.Remove(subscriptionToRemove);
                if (subscriptions.Count == 0)
                {
                    _subscriptions.TryRemove(kvp.Key, out _);
                }
                break;
            }
        }
    }

    /// <inheritdoc />
    public void UnsubscribeAll<TEvent>() where TEvent : class
    {
        var eventType = typeof(TEvent);
        _subscriptions.TryRemove(eventType, out _);
    }

    /// <inheritdoc />
    public void Clear()
    {
        _subscriptions.Clear();
    }
}

/// <summary>
/// 事件订阅基类
/// </summary>
internal abstract class EventSubscription
{
    public string Id { get; }
    public int Priority { get; }

    protected EventSubscription(int priority = 100)
    {
        Id = Guid.NewGuid().ToString();
        Priority = priority;
    }

    public abstract Task HandleAsync(object @event);
    public abstract bool ShouldHandle(object @event);
}

/// <summary>
/// 事件订阅实现
/// </summary>
/// <typeparam name="TEvent">事件类型</typeparam>
internal class EventSubscription<TEvent> : EventSubscription where TEvent : class
{
    private readonly IEventHandler<TEvent> _handler;

    public EventSubscription(IEventHandler<TEvent> handler) : base(handler.Priority)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
    }

    public override async Task HandleAsync(object @event)
    {
        if (@event is TEvent typedEvent)
        {
            await _handler.HandleAsync(typedEvent);
        }
    }

    public override bool ShouldHandle(object @event)
    {
        return @event is TEvent typedEvent && _handler.ShouldHandle(typedEvent);
    }
}

/// <summary>
/// 委托事件处理器
/// </summary>
/// <typeparam name="TEvent">事件类型</typeparam>
internal class DelegateEventHandler<TEvent> : IEventHandler<TEvent> where TEvent : class
{
    private readonly Func<TEvent, Task> _handler;

    public DelegateEventHandler(Func<TEvent, Task> handler)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
    }

    public string Name => "Delegate";

    public int Priority => 100;

    public async Task HandleAsync(TEvent @event)
    {
        await _handler(@event);
    }

    public bool ShouldHandle(TEvent @event)
    {
        return true;
    }
}

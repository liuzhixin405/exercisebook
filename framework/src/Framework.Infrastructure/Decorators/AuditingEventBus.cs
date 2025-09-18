using Framework.Core.Abstractions.Events;

namespace Framework.Infrastructure.Decorators;

/// <summary>
/// 审计事件总线 - 装饰器模式
/// 为事件总线添加审计功能
/// </summary>
public class AuditingEventBus : IEventBus
{
    private readonly IEventBus _innerEventBus;
    private readonly IAuditLogger _auditLogger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="innerEventBus">内部事件总线</param>
    /// <param name="auditLogger">审计日志记录器</param>
    public AuditingEventBus(IEventBus innerEventBus, IAuditLogger auditLogger)
    {
        _innerEventBus = innerEventBus ?? throw new ArgumentNullException(nameof(innerEventBus));
        _auditLogger = auditLogger ?? throw new ArgumentNullException(nameof(auditLogger));
    }

    /// <inheritdoc />
    public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : class
    {
        try
        {
            await _auditLogger.LogAuditAsync("EventPublish", new { EventType = typeof(TEvent).Name, Event = @event });
            await _innerEventBus.PublishAsync(@event);
            await _auditLogger.LogAuditAsync("EventPublished", new { EventType = typeof(TEvent).Name, Event = @event });
        }
        catch (Exception ex)
        {
            await _auditLogger.LogAuditAsync("EventPublishFailed", new { EventType = typeof(TEvent).Name, Event = @event, Error = ex.Message });
            throw;
        }
    }

    /// <inheritdoc />
    public void Publish<TEvent>(TEvent @event) where TEvent : class
    {
        try
        {
            _auditLogger.LogAuditAsync("EventPublish", new { EventType = typeof(TEvent).Name, Event = @event }).GetAwaiter().GetResult();
            _innerEventBus.Publish(@event);
            _auditLogger.LogAuditAsync("EventPublished", new { EventType = typeof(TEvent).Name, Event = @event }).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            _auditLogger.LogAuditAsync("EventPublishFailed", new { EventType = typeof(TEvent).Name, Event = @event, Error = ex.Message }).GetAwaiter().GetResult();
            throw;
        }
    }

    /// <inheritdoc />
    public string Subscribe<TEvent>(IEventHandler<TEvent> handler) where TEvent : class
    {
        var subscriptionId = _innerEventBus.Subscribe(handler);
        _auditLogger.LogAuditAsync("EventSubscribe", new { EventType = typeof(TEvent).Name, HandlerType = handler.GetType().Name, SubscriptionId = subscriptionId }).GetAwaiter().GetResult();
        return subscriptionId;
    }

    /// <inheritdoc />
    public string Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : class
    {
        var subscriptionId = _innerEventBus.Subscribe(handler);
        _auditLogger.LogAuditAsync("EventSubscribe", new { EventType = typeof(TEvent).Name, HandlerType = "Delegate", SubscriptionId = subscriptionId }).GetAwaiter().GetResult();
        return subscriptionId;
    }

    /// <inheritdoc />
    public void Unsubscribe(string subscriptionId)
    {
        _innerEventBus.Unsubscribe(subscriptionId);
        _auditLogger.LogAuditAsync("EventUnsubscribe", new { SubscriptionId = subscriptionId }).GetAwaiter().GetResult();
    }

    /// <inheritdoc />
    public void UnsubscribeAll<TEvent>() where TEvent : class
    {
        _innerEventBus.UnsubscribeAll<TEvent>();
        _auditLogger.LogAuditAsync("EventUnsubscribeAll", new { EventType = typeof(TEvent).Name }).GetAwaiter().GetResult();
    }

    /// <inheritdoc />
    public void Clear()
    {
        _innerEventBus.Clear();
        _auditLogger.LogAuditAsync("EventBusClear", null).GetAwaiter().GetResult();
    }
}

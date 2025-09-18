namespace Framework.Core.Abstractions.Events;

/// <summary>
/// 事件总线接口 - 观察者模式
/// 提供事件发布和订阅的抽象
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// 发布事件
    /// </summary>
    /// <typeparam name="TEvent">事件类型</typeparam>
    /// <param name="event">事件实例</param>
    /// <returns>任务</returns>
    Task PublishAsync<TEvent>(TEvent @event) where TEvent : class;

    /// <summary>
    /// 发布事件（同步）
    /// </summary>
    /// <typeparam name="TEvent">事件类型</typeparam>
    /// <param name="event">事件实例</param>
    void Publish<TEvent>(TEvent @event) where TEvent : class;

    /// <summary>
    /// 订阅事件
    /// </summary>
    /// <typeparam name="TEvent">事件类型</typeparam>
    /// <param name="handler">事件处理器</param>
    /// <returns>订阅标识</returns>
    string Subscribe<TEvent>(IEventHandler<TEvent> handler) where TEvent : class;

    /// <summary>
    /// 订阅事件（委托）
    /// </summary>
    /// <typeparam name="TEvent">事件类型</typeparam>
    /// <param name="handler">事件处理委托</param>
    /// <returns>订阅标识</returns>
    string Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : class;

    /// <summary>
    /// 取消订阅
    /// </summary>
    /// <param name="subscriptionId">订阅标识</param>
    void Unsubscribe(string subscriptionId);

    /// <summary>
    /// 取消订阅事件类型的所有处理器
    /// </summary>
    /// <typeparam name="TEvent">事件类型</typeparam>
    void UnsubscribeAll<TEvent>() where TEvent : class;

    /// <summary>
    /// 清空所有订阅
    /// </summary>
    void Clear();
}

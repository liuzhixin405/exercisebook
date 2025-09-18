namespace Framework.Core.Abstractions.Events;

/// <summary>
/// 事件处理器接口 - 观察者模式
/// 提供事件处理的抽象
/// </summary>
/// <typeparam name="TEvent">事件类型</typeparam>
public interface IEventHandler<in TEvent> where TEvent : class
{
    /// <summary>
    /// 处理器名称
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 处理器优先级（数字越小优先级越高）
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// 处理事件
    /// </summary>
    /// <param name="event">事件实例</param>
    /// <returns>任务</returns>
    Task HandleAsync(TEvent @event);

    /// <summary>
    /// 是否应该处理此事件
    /// </summary>
    /// <param name="event">事件实例</param>
    /// <returns>是否应该处理</returns>
    bool ShouldHandle(TEvent @event);
}

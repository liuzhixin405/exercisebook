namespace Framework.Core.Abstractions.States;

/// <summary>
/// 状态转换处理器接口 - 状态模式
/// 提供状态转换处理的抽象
/// </summary>
/// <typeparam name="TFromState">源状态类型</typeparam>
/// <typeparam name="TToState">目标状态类型</typeparam>
public interface IStateTransitionHandler<in TFromState, in TToState>
    where TFromState : class, IState
    where TToState : class, IState
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
    /// 处理状态转换
    /// </summary>
    /// <param name="fromState">源状态</param>
    /// <param name="toState">目标状态</param>
    /// <param name="context">状态上下文</param>
    /// <returns>是否允许转换</returns>
    Task<bool> HandleTransitionAsync(TFromState fromState, TToState toState, IStateContext context);

    /// <summary>
    /// 是否应该处理此转换
    /// </summary>
    /// <param name="fromState">源状态</param>
    /// <param name="toState">目标状态</param>
    /// <returns>是否应该处理</returns>
    bool ShouldHandle(TFromState fromState, TToState toState);
}

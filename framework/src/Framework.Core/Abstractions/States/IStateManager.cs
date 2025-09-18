namespace Framework.Core.Abstractions.States;

/// <summary>
/// 状态管理器接口 - 状态模式
/// 提供状态管理的抽象
/// </summary>
public interface IStateManager
{
    /// <summary>
    /// 获取当前状态
    /// </summary>
    /// <typeparam name="TState">状态类型</typeparam>
    /// <returns>当前状态</returns>
    TState? GetCurrentState<TState>() where TState : class, IState;

    /// <summary>
    /// 设置状态
    /// </summary>
    /// <typeparam name="TState">状态类型</typeparam>
    /// <param name="state">状态实例</param>
    void SetState<TState>(TState state) where TState : class, IState;

    /// <summary>
    /// 转换到新状态
    /// </summary>
    /// <typeparam name="TState">状态类型</typeparam>
    /// <param name="state">新状态</param>
    /// <returns>是否转换成功</returns>
    Task<bool> TransitionToAsync<TState>(TState state) where TState : class, IState;

    /// <summary>
    /// 注册状态转换处理器
    /// </summary>
    /// <typeparam name="TFromState">源状态类型</typeparam>
    /// <typeparam name="TToState">目标状态类型</typeparam>
    /// <param name="handler">转换处理器</param>
    void RegisterTransitionHandler<TFromState, TToState>(IStateTransitionHandler<TFromState, TToState> handler)
        where TFromState : class, IState
        where TToState : class, IState;

    /// <summary>
    /// 获取状态历史
    /// </summary>
    /// <returns>状态历史</returns>
    IEnumerable<IState> GetStateHistory();

    /// <summary>
    /// 清空状态历史
    /// </summary>
    void ClearHistory();

    /// <summary>
    /// 状态变化事件
    /// </summary>
    event EventHandler<StateChangedEventArgs>? StateChanged;
}

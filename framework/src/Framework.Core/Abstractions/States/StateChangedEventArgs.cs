namespace Framework.Core.Abstractions.States;

/// <summary>
/// 状态变化事件参数
/// </summary>
public class StateChangedEventArgs : EventArgs
{
    /// <summary>
    /// 源状态
    /// </summary>
    public IState? FromState { get; }

    /// <summary>
    /// 目标状态
    /// </summary>
    public IState ToState { get; }

    /// <summary>
    /// 变化时间
    /// </summary>
    public DateTime Timestamp { get; }

    /// <summary>
    /// 状态上下文
    /// </summary>
    public IStateContext Context { get; }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="fromState">源状态</param>
    /// <param name="toState">目标状态</param>
    /// <param name="context">状态上下文</param>
    public StateChangedEventArgs(IState? fromState, IState toState, IStateContext context)
    {
        FromState = fromState;
        ToState = toState;
        Context = context;
        Timestamp = DateTime.UtcNow;
    }
}

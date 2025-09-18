namespace Framework.Core.Abstractions.States;

/// <summary>
/// 状态接口 - 状态模式
/// 提供状态的抽象
/// </summary>
public interface IState
{
    /// <summary>
    /// 状态名称
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 状态标识
    /// </summary>
    string Id { get; }

    /// <summary>
    /// 进入状态
    /// </summary>
    /// <param name="context">状态上下文</param>
    /// <returns>任务</returns>
    Task OnEnterAsync(IStateContext context);

    /// <summary>
    /// 退出状态
    /// </summary>
    /// <param name="context">状态上下文</param>
    /// <returns>任务</returns>
    Task OnExitAsync(IStateContext context);

    /// <summary>
    /// 处理状态
    /// </summary>
    /// <param name="context">状态上下文</param>
    /// <returns>任务</returns>
    Task OnProcessAsync(IStateContext context);

    /// <summary>
    /// 是否可以转换到指定状态
    /// </summary>
    /// <param name="targetState">目标状态</param>
    /// <returns>是否可以转换</returns>
    bool CanTransitionTo(IState targetState);
}

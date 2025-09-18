namespace Framework.Core.Abstractions.Commands;

/// <summary>
/// 命令处理器接口 - 命令模式
/// 提供命令处理的抽象
/// </summary>
/// <typeparam name="TCommand">命令类型</typeparam>
public interface ICommandHandler<in TCommand> where TCommand : class, ICommand
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
    /// 处理命令
    /// </summary>
    /// <param name="command">命令实例</param>
    /// <returns>任务</returns>
    Task HandleAsync(TCommand command);

    /// <summary>
    /// 是否应该处理此命令
    /// </summary>
    /// <param name="command">命令实例</param>
    /// <returns>是否应该处理</returns>
    bool ShouldHandle(TCommand command);
}

/// <summary>
/// 带结果的命令处理器接口 - 命令模式
/// </summary>
/// <typeparam name="TCommand">命令类型</typeparam>
/// <typeparam name="TResult">结果类型</typeparam>
public interface ICommandHandler<in TCommand, TResult> where TCommand : class, ICommand<TResult>
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
    /// 处理命令并返回结果
    /// </summary>
    /// <param name="command">命令实例</param>
    /// <returns>命令结果</returns>
    Task<TResult> HandleAsync(TCommand command);

    /// <summary>
    /// 是否应该处理此命令
    /// </summary>
    /// <param name="command">命令实例</param>
    /// <returns>是否应该处理</returns>
    bool ShouldHandle(TCommand command);
}

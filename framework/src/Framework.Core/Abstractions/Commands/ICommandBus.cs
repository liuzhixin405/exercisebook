namespace Framework.Core.Abstractions.Commands;

/// <summary>
/// 命令总线接口 - 命令模式
/// 提供命令发送和处理的抽象
/// </summary>
public interface ICommandBus
{
    /// <summary>
    /// 发送命令
    /// </summary>
    /// <typeparam name="TCommand">命令类型</typeparam>
    /// <param name="command">命令实例</param>
    /// <returns>任务</returns>
    Task SendAsync<TCommand>(TCommand command) where TCommand : class, ICommand;

    /// <summary>
    /// 发送命令并返回结果
    /// </summary>
    /// <typeparam name="TCommand">命令类型</typeparam>
    /// <typeparam name="TResult">结果类型</typeparam>
    /// <param name="command">命令实例</param>
    /// <returns>命令结果</returns>
    Task<TResult> SendAsync<TCommand, TResult>(TCommand command) 
        where TCommand : class, ICommand<TResult>;

    /// <summary>
    /// 注册命令处理器
    /// </summary>
    /// <typeparam name="TCommand">命令类型</typeparam>
    /// <param name="handler">命令处理器</param>
    /// <returns>命令总线</returns>
    ICommandBus RegisterHandler<TCommand>(ICommandHandler<TCommand> handler) 
        where TCommand : class, ICommand;

    /// <summary>
    /// 注册命令处理器（带结果）
    /// </summary>
    /// <typeparam name="TCommand">命令类型</typeparam>
    /// <typeparam name="TResult">结果类型</typeparam>
    /// <param name="handler">命令处理器</param>
    /// <returns>命令总线</returns>
    ICommandBus RegisterHandler<TCommand, TResult>(ICommandHandler<TCommand, TResult> handler) 
        where TCommand : class, ICommand<TResult>;

    /// <summary>
    /// 取消注册命令处理器
    /// </summary>
    /// <typeparam name="TCommand">命令类型</typeparam>
    /// <returns>命令总线</returns>
    ICommandBus UnregisterHandler<TCommand>() where TCommand : class, ICommand;

    /// <summary>
    /// 清空所有处理器
    /// </summary>
    /// <returns>命令总线</returns>
    ICommandBus Clear();
}

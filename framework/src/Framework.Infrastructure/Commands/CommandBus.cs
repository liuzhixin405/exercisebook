using Framework.Core.Abstractions.Commands;
using System.Collections.Concurrent;

namespace Framework.Infrastructure.Commands;

/// <summary>
/// 命令总线实现 - 命令模式
/// 提供命令发送和处理的实现
/// </summary>
public class CommandBus : ICommandBus
{
    private readonly ConcurrentDictionary<Type, ICommandHandler> _handlers;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="serviceProvider">服务提供者</param>
    public CommandBus(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _handlers = new ConcurrentDictionary<Type, ICommandHandler>();
    }

    /// <inheritdoc />
    public async Task SendAsync<TCommand>(TCommand command) where TCommand : class, ICommand
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command));

        var commandType = typeof(TCommand);
        if (!_handlers.TryGetValue(commandType, out var handler))
        {
            throw new InvalidOperationException($"No handler registered for command type {commandType.Name}");
        }

        if (handler is ICommandHandler<TCommand> typedHandler)
        {
            if (typedHandler.ShouldHandle(command))
            {
                await typedHandler.HandleAsync(command);
            }
        }
    }

    /// <inheritdoc />
    public async Task<TResult> SendAsync<TCommand, TResult>(TCommand command) 
        where TCommand : class, ICommand<TResult>
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command));

        var commandType = typeof(TCommand);
        if (!_handlers.TryGetValue(commandType, out var handler))
        {
            throw new InvalidOperationException($"No handler registered for command type {commandType.Name}");
        }

        if (handler is ICommandHandler<TCommand, TResult> typedHandler)
        {
            if (typedHandler.ShouldHandle(command))
            {
                return await typedHandler.HandleAsync(command);
            }
        }

        throw new InvalidOperationException($"Handler for command type {commandType.Name} does not support result type {typeof(TResult).Name}");
    }

    /// <inheritdoc />
    public ICommandBus RegisterHandler<TCommand>(ICommandHandler<TCommand> handler) 
        where TCommand : class, ICommand
    {
        if (handler == null)
            throw new ArgumentNullException(nameof(handler));

        var commandType = typeof(TCommand);
        _handlers.AddOrUpdate(commandType, handler, (key, existing) => handler);
        return this;
    }

    /// <inheritdoc />
    public ICommandBus RegisterHandler<TCommand, TResult>(ICommandHandler<TCommand, TResult> handler) 
        where TCommand : class, ICommand<TResult>
    {
        if (handler == null)
            throw new ArgumentNullException(nameof(handler));

        var commandType = typeof(TCommand);
        _handlers.AddOrUpdate(commandType, handler, (key, existing) => handler);
        return this;
    }

    /// <inheritdoc />
    public ICommandBus UnregisterHandler<TCommand>() where TCommand : class, ICommand
    {
        var commandType = typeof(TCommand);
        _handlers.TryRemove(commandType, out _);
        return this;
    }

    /// <inheritdoc />
    public ICommandBus Clear()
    {
        _handlers.Clear();
        return this;
    }
}

/// <summary>
/// 命令处理器基类
/// </summary>
internal abstract class CommandHandler
{
    public abstract string Name { get; }
    public abstract int Priority { get; }
    public abstract Task HandleAsync(object command);
    public abstract bool ShouldHandle(object command);
}

/// <summary>
/// 命令处理器接口（非泛型）
/// </summary>
internal interface ICommandHandler
{
    string Name { get; }
    int Priority { get; }
    Task HandleAsync(object command);
    bool ShouldHandle(object command);
}

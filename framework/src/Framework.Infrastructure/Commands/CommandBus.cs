using Framework.Core.Abstractions.Commands;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace Framework.Infrastructure.Commands;

/// <summary>
/// 命令总线实现 - 命令模式
/// 提供命令发送和处理的实现
/// </summary>
public class CommandBus : ICommandBus
{
    private readonly ConcurrentDictionary<Type, IInternalCommandHandler> _handlers;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="serviceProvider">服务提供者</param>
    public CommandBus(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _handlers = new ConcurrentDictionary<Type, IInternalCommandHandler>();
    }

    private void EnsureHandlerRegisteredFromServiceProvider(Type commandType)
    {
        if (_handlers.ContainsKey(commandType))
            return;

        try
        {
            // Try non-result handler ICommandHandler<TCommand>
            var handlerInterface = typeof(ICommandHandler<>).MakeGenericType(commandType);

            // If root provider can resolve a singleton/shared instance, register it directly
            var rootObj = _serviceProvider.GetService(handlerInterface);
            if (rootObj != null)
            {
                var registerMethod = typeof(CommandBus).GetMethod(nameof(RegisterHandler), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                var genericRegister = registerMethod!.MakeGenericMethod(commandType);
                genericRegister.Invoke(this, new[] { rootObj });
                return;
            }

            // Otherwise check a scoped provider: if a handler is registered as scoped/transient, register a proxy that resolves per-invocation
            using (var scope = _serviceProvider.CreateScope())
            {
                var scopedObj = scope.ServiceProvider.GetService(handlerInterface);
                if (scopedObj != null)
                {
                    var proxyType = typeof(ServiceProviderHandlerProxy<>).MakeGenericType(commandType);
                    var proxy = Activator.CreateInstance(proxyType, _serviceProvider);
                    if (proxy != null)
                    {
                        var registerMethod = typeof(CommandBus).GetMethod(nameof(RegisterHandler), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                        var genericRegister = registerMethod!.MakeGenericMethod(commandType);
                        genericRegister.Invoke(this, new[] { proxy });
                        return;
                    }
                }
            }

            // Try result handler ICommandHandler<TCommand, TResult> - handled in SendAsync with TResult when needed
        }
        catch(Exception ex)
        {
            // Log or handle exception as needed
            Console.WriteLine($"Error resolving handler for command type {commandType.Name}: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task SendAsync<TCommand>(TCommand command) where TCommand : class, ICommand
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command));

        var commandType = typeof(TCommand);
        if (!_handlers.TryGetValue(commandType, out var handler))
        {
            // attempt to resolve handler from DI and register
            EnsureHandlerRegisteredFromServiceProvider(commandType);

            if (!_handlers.TryGetValue(commandType, out handler))
            {
                throw new InvalidOperationException($"No handler registered for command type {commandType.Name}");
            }
        }

        if (handler.ShouldHandle(command))
        {
            await handler.HandleAsync(command);
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
            // attempt to resolve handler from DI and register
            EnsureHandlerRegisteredFromServiceProvider(commandType);

            if (!_handlers.TryGetValue(commandType, out handler))
            {
                // Try to resolve ICommandHandler<TCommand, TResult> specifically
                try
                {
                    var handlerInterface = typeof(ICommandHandler<,>).MakeGenericType(commandType, typeof(TResult));

                    var rootObj = _serviceProvider.GetService(handlerInterface);
                    if (rootObj != null)
                    {
                        var registerMethod = typeof(CommandBus).GetMethod(nameof(RegisterHandler), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                        var genericRegister = registerMethod!.MakeGenericMethod(commandType, typeof(TResult));
                        genericRegister.Invoke(this, new[] { rootObj });

                        if (!_handlers.TryGetValue(commandType, out handler))
                        {
                            throw new InvalidOperationException($"No handler registered for command type {commandType.Name}");
                        }
                    }
                    else
                    {
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var scopedObj = scope.ServiceProvider.GetService(handlerInterface);
                            if (scopedObj != null)
                            {
                                var proxyType = typeof(ServiceProviderHandlerWithResultProxy<,>).MakeGenericType(commandType, typeof(TResult));
                                var proxy = Activator.CreateInstance(proxyType, _serviceProvider);
                                if (proxy != null)
                                {
                                    var registerMethod = typeof(CommandBus).GetMethod(nameof(RegisterHandler), System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                                    var genericRegister = registerMethod!.MakeGenericMethod(commandType, typeof(TResult));
                                    genericRegister.Invoke(this, new[] { proxy });

                                    if (!_handlers.TryGetValue(commandType, out handler))
                                    {
                                        throw new InvalidOperationException($"No handler registered for command type {commandType.Name}");
                                    }
                                }
                                else
                                {
                                    throw new InvalidOperationException($"No handler registered for command type {commandType.Name}");
                                }
                            }
                            else
                            {
                                throw new InvalidOperationException($"No handler registered for command type {commandType.Name}");
                            }
                        }
                    }
                }
                catch
                {
                    throw new InvalidOperationException($"No handler registered for command type {commandType.Name}");
                }
            }
        }

        if (handler.ShouldHandle(command))
        {
            var resultObj = await handler.HandleAsync(command);
            if (resultObj is TResult result)
                return result;

            // allow null/default cast
            return default!;
        }

        throw new InvalidOperationException($"Handler for command type {commandType.Name} did not handle the command or returned incompatible result.");
    }

    /// <inheritdoc />
    public ICommandBus RegisterHandler<TCommand>(ICommandHandler<TCommand> handler)
        where TCommand : class, ICommand
    {
        if (handler == null)
            throw new ArgumentNullException(nameof(handler));

        var commandType = typeof(TCommand);
        var adapter = new CommandHandlerAdapter<TCommand>(handler);
        _handlers.AddOrUpdate(commandType, adapter, (key, existing) => adapter);
        return this;
    }

    /// <inheritdoc />
    public ICommandBus RegisterHandler<TCommand, TResult>(ICommandHandler<TCommand, TResult> handler)
        where TCommand : class, ICommand<TResult>
    {
        if (handler == null)
            throw new ArgumentNullException(nameof(handler));

        var commandType = typeof(TCommand);
        var adapter = new CommandHandlerWithResultAdapter<TCommand, TResult>(handler);
        _handlers.AddOrUpdate(commandType, adapter, (key, existing) => adapter);
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
/// 内部命令处理器适配器，统一返回 object? 结果并接收 object 参数
/// </summary>
internal interface IInternalCommandHandler
{
    Task<object?> HandleAsync(object command);
    bool ShouldHandle(object command);
}

internal class CommandHandlerAdapter<TCommand> : IInternalCommandHandler where TCommand : class, ICommand
{
    private readonly ICommandHandler<TCommand> _inner;

    public CommandHandlerAdapter(ICommandHandler<TCommand> inner)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
    }

    public async Task<object?> HandleAsync(object command)
    {
        if (command is TCommand typed)
        {
            await _inner.HandleAsync(typed);
            return null;
        }

        throw new InvalidOperationException($"Command type mismatch. Expected {typeof(TCommand).Name}");
    }

    public bool ShouldHandle(object command)
    {
        if (command is TCommand typed)
        {
            return _inner.ShouldHandle(typed);
        }
        return false;
    }
}

internal class CommandHandlerWithResultAdapter<TCommand, TResult> : IInternalCommandHandler where TCommand : class, ICommand<TResult>
{
    private readonly ICommandHandler<TCommand, TResult> _inner;

    public CommandHandlerWithResultAdapter(ICommandHandler<TCommand, TResult> inner)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
    }

    public async Task<object?> HandleAsync(object command)
    {
        if (command is TCommand typed)
        {
            var result = await _inner.HandleAsync(typed);
            return (object?)result;
        }

        throw new InvalidOperationException($"Command type mismatch. Expected {typeof(TCommand).Name}");
    }

    public bool ShouldHandle(object command)
    {
        if (command is TCommand typed)
        {
            return _inner.ShouldHandle(typed);
        }
        return false;
    }
}

// Proxy implementations that resolve actual handlers from IServiceProvider per invocation
internal class ServiceProviderHandlerProxy<TCommand> : ICommandHandler<TCommand> where TCommand : class, ICommand
{
    private readonly IServiceProvider _provider;

    public ServiceProviderHandlerProxy(IServiceProvider provider)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    public string Name => "ServiceProviderProxy";

    public int Priority => 100;

    public async Task HandleAsync(TCommand command)
    {
        using var scope = _provider.CreateScope();
        var handler = (ICommandHandler<TCommand>)scope.ServiceProvider.GetRequiredService(typeof(ICommandHandler<TCommand>));
        await handler.HandleAsync(command);
    }

    public bool ShouldHandle(TCommand command)
    {
        using var scope = _provider.CreateScope();
        var handler = (ICommandHandler<TCommand>)scope.ServiceProvider.GetRequiredService(typeof(ICommandHandler<TCommand>));
        return handler.ShouldHandle(command);
    }
}

internal class ServiceProviderHandlerWithResultProxy<TCommand, TResult> : ICommandHandler<TCommand, TResult> where TCommand : class, ICommand<TResult>
{
    private readonly IServiceProvider _provider;

    public ServiceProviderHandlerWithResultProxy(IServiceProvider provider)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    public string Name => "ServiceProviderProxy";

    public int Priority => 100;

    public async Task<TResult> HandleAsync(TCommand command)
    {
        using var scope = _provider.CreateScope();
        var handler = (ICommandHandler<TCommand, TResult>)scope.ServiceProvider.GetRequiredService(typeof(ICommandHandler<TCommand, TResult>));
        return await handler.HandleAsync(command);
    }

    public bool ShouldHandle(TCommand command)
    {
        using var scope = _provider.CreateScope();
        var handler = (ICommandHandler<TCommand, TResult>)scope.ServiceProvider.GetRequiredService(typeof(ICommandHandler<TCommand, TResult>));
        return handler.ShouldHandle(command);
    }
}

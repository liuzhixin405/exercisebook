using Framework.Core.Abstractions.Commands;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

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
        
        // 在构造时立即注册所有命令处理器
        AutoRegisterHandlers();
    }

    /// <summary>
    /// 自动发现并注册命令处理器
    /// </summary>
    private void AutoRegisterHandlers()
    {
        try
        {
            Console.WriteLine("开始自动注册命令处理器...");
            
            // 通过反射查找所有已注册的 ICommandHandler 服务
            var allServices = new List<object>();
            
            // 获取所有程序集中实现了 ICommandHandler 接口的类型
            var allTypes = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                .SelectMany(a => 
                {
                    try { return a.GetTypes(); }
                    catch { return new Type[0]; }
                })
                .Where(t => t.IsClass && !t.IsAbstract)
                .Where(t => t.GetInterfaces().Any(i => 
                    i.IsGenericType && 
                    (i.GetGenericTypeDefinition() == typeof(ICommandHandler<>) ||
                     i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>))))
                .ToList();

            Console.WriteLine($"找到 {allTypes.Count} 个命令处理器类型");

            foreach (var type in allTypes)
            {
                try
                {
                    // 尝试通过泛型接口获取服务
                    var interfaces = type.GetInterfaces()
                        .Where(i => i.IsGenericType && 
                                   (i.GetGenericTypeDefinition() == typeof(ICommandHandler<>) ||
                                    i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)))
                        .ToList();

                    foreach (var interfaceType in interfaces)
                    {
                        try
                        {
                            var service = _serviceProvider.GetService(interfaceType);
                            if (service != null)
                            {
                                allServices.Add(service);
                                Console.WriteLine($"通过泛型接口找到服务: {type.Name} -> {interfaceType.Name}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"通过泛型接口获取服务 {type.Name} 时出错: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"处理类型 {type.Name} 时出错: {ex.Message}");
                }
            }

            Console.WriteLine($"总共发现 {allServices.Count} 个命令处理器");

            foreach (var handler in allServices)
            {
                var handlerType = handler.GetType();
                Console.WriteLine($"处理命令处理器: {handlerType.Name}");
                
                var interfaces = handlerType.GetInterfaces()
                    .Where(i => i.IsGenericType && 
                               (i.GetGenericTypeDefinition() == typeof(ICommandHandler<>) ||
                                i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)))
                    .ToList();

                foreach (var interfaceType in interfaces)
                {
                    var commandType = interfaceType.GetGenericArguments()[0];
                    Console.WriteLine($"注册命令类型: {commandType.Name} -> 处理器: {handlerType.Name}");
                    var wrapper = new CommandHandlerWrapper(handler, commandType);
                    _handlers.AddOrUpdate(commandType, wrapper, (key, existing) => wrapper);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"自动注册命令处理器时出错: {ex.Message}");
            Console.WriteLine($"堆栈跟踪: {ex.StackTrace}");
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
            var registeredTypes = string.Join(", ", _handlers.Keys.Select(t => t.Name));
            throw new InvalidOperationException(
                $"No handler registered for command type '{commandType.Name}'. " +
                $"Registered command types: [{registeredTypes}]. " +
                $"Please ensure the command handler is registered using AddCommandHandler<{commandType.Name}, {commandType.Name}Handler>()");
        }

        try
        {
            if (handler.ShouldHandle(command))
            {
                await handler.HandleAsync(command);
            }
            else
            {
                Console.WriteLine($"Command handler '{handler.Name}' determined it should not handle command '{commandType.Name}'");
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Error executing command '{commandType.Name}' with handler '{handler.Name}': {ex.Message}", ex);
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
        var wrapper = new CommandHandlerWrapper<TCommand>(handler);
        _handlers.AddOrUpdate(commandType, wrapper, (key, existing) => wrapper);
        return this;
    }

    /// <inheritdoc />
    public ICommandBus RegisterHandler<TCommand, TResult>(ICommandHandler<TCommand, TResult> handler) 
        where TCommand : class, ICommand<TResult>
    {
        if (handler == null)
            throw new ArgumentNullException(nameof(handler));

        var commandType = typeof(TCommand);
        var wrapper = new CommandHandlerWrapper<TCommand, TResult>(handler);
        _handlers.AddOrUpdate(commandType, wrapper, (key, existing) => wrapper);
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

/// <summary>
/// 命令处理器包装器
/// </summary>
/// <typeparam name="TCommand">命令类型</typeparam>
internal class CommandHandlerWrapper<TCommand> : ICommandHandler where TCommand : class, ICommand
{
    private readonly ICommandHandler<TCommand> _handler;

    public CommandHandlerWrapper(ICommandHandler<TCommand> handler)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
    }

    public string Name => _handler.Name;
    public int Priority => _handler.Priority;

    public async Task HandleAsync(object command)
    {
        if (command is TCommand typedCommand)
        {
            await _handler.HandleAsync(typedCommand);
        }
    }

    public bool ShouldHandle(object command)
    {
        return command is TCommand typedCommand && _handler.ShouldHandle(typedCommand);
    }
}

/// <summary>
/// 命令处理器包装器（带结果）
/// </summary>
/// <typeparam name="TCommand">命令类型</typeparam>
/// <typeparam name="TResult">结果类型</typeparam>
internal class CommandHandlerWrapper<TCommand, TResult> : ICommandHandler where TCommand : class, ICommand<TResult>
{
    private readonly ICommandHandler<TCommand, TResult> _handler;

    public CommandHandlerWrapper(ICommandHandler<TCommand, TResult> handler)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
    }

    public string Name => _handler.Name;
    public int Priority => _handler.Priority;

    public async Task HandleAsync(object command)
    {
        if (command is TCommand typedCommand)
        {
            await _handler.HandleAsync(typedCommand);
        }
    }

    public bool ShouldHandle(object command)
    {
        return command is TCommand typedCommand && _handler.ShouldHandle(typedCommand);
    }
}

/// <summary>
/// 通用命令处理器包装器
/// </summary>
internal class CommandHandlerWrapper : ICommandHandler
{
    private readonly object _handler;
    private readonly Type _commandType;

    public CommandHandlerWrapper(object handler, Type commandType)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        _commandType = commandType ?? throw new ArgumentNullException(nameof(commandType));
    }

    public string Name => GetPropertyValue<string>("Name");
    public int Priority => GetPropertyValue<int>("Priority");

    public async Task HandleAsync(object command)
    {
        if (command?.GetType() == _commandType)
        {
            var handleMethod = _handler.GetType().GetMethod("HandleAsync", new[] { _commandType });
            if (handleMethod != null)
            {
                var task = (Task)handleMethod.Invoke(_handler, new[] { command });
                if (task != null)
                {
                    await task;
                }
            }
        }
    }

    public bool ShouldHandle(object command)
    {
        if (command?.GetType() != _commandType)
            return false;

        var shouldHandleMethod = _handler.GetType().GetMethod("ShouldHandle", new[] { _commandType });
        if (shouldHandleMethod != null)
        {
            return (bool)shouldHandleMethod.Invoke(_handler, new[] { command });
        }

        return true;
    }

    private T GetPropertyValue<T>(string propertyName)
    {
        var property = _handler.GetType().GetProperty(propertyName);
        if (property != null)
        {
            return (T)property.GetValue(_handler);
        }
        return default(T);
    }
}
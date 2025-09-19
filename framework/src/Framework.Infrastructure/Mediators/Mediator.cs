using System.Collections.Concurrent;

namespace Framework.Infrastructure.Mediators;

/// <summary>
/// 中介者实现 - 中介者模式
/// 提供对象间通信的实现
/// </summary>
public class Mediator : IMediator
{
    private readonly ConcurrentDictionary<Type, IMessageHandler> _handlers;

    /// <summary>
    /// 构造函数
    /// </summary>
    public Mediator()
    {
        _handlers = new ConcurrentDictionary<Type, IMessageHandler>();
    }

    /// <inheritdoc />
    public async Task SendAsync<TMessage>(TMessage message) where TMessage : class
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        var messageType = typeof(TMessage);
        if (!_handlers.TryGetValue(messageType, out var handler))
        {
            throw new InvalidOperationException($"No handler registered for message type {messageType.Name}");
        }

        if (handler is IMessageHandler<TMessage> typedHandler)
        {
            if (typedHandler.ShouldHandle(message))
            {
                await typedHandler.HandleAsync(message);
            }
        }
    }

    /// <inheritdoc />
    public async Task<TResult> SendAsync<TMessage, TResult>(TMessage message) where TMessage : class
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        var messageType = typeof(TMessage);
        if (!_handlers.TryGetValue(messageType, out var handler))
        {
            throw new InvalidOperationException($"No handler registered for message type {messageType.Name}");
        }

        if (handler is IMessageHandler<TMessage, TResult> typedHandler)
        {
            if (typedHandler.ShouldHandle(message))
            {
                return await typedHandler.HandleAsync(message);
            }
        }

        throw new InvalidOperationException($"Handler for message type {messageType.Name} does not support result type {typeof(TResult).Name}");
    }

    /// <inheritdoc />
    public IMediator RegisterHandler<TMessage>(IMessageHandler<TMessage> handler) where TMessage : class
    {
        if (handler == null)
            throw new ArgumentNullException(nameof(handler));

        var messageType = typeof(TMessage);
        var wrapper = new MessageHandlerWrapper<TMessage>(handler);
        _handlers.AddOrUpdate(messageType, wrapper, (key, existing) => wrapper);
        return this;
    }

    /// <inheritdoc />
    public IMediator RegisterHandler<TMessage, TResult>(IMessageHandler<TMessage, TResult> handler) where TMessage : class
    {
        if (handler == null)
            throw new ArgumentNullException(nameof(handler));

        var messageType = typeof(TMessage);
        var wrapper = new MessageHandlerWrapper<TMessage, TResult>(handler);
        _handlers.AddOrUpdate(messageType, wrapper, (key, existing) => wrapper);
        return this;
    }

    /// <inheritdoc />
    public IMediator UnregisterHandler<TMessage>() where TMessage : class
    {
        var messageType = typeof(TMessage);
        _handlers.TryRemove(messageType, out _);
        return this;
    }
}

/// <summary>
/// 消息处理器基类
/// </summary>
internal abstract class MessageHandler
{
    public abstract string Name { get; }
    public abstract int Priority { get; }
    public abstract Task HandleAsync(object message);
    public abstract bool ShouldHandle(object message);
}

/// <summary>
/// 消息处理器接口（非泛型）
/// </summary>
internal interface IMessageHandler
{
    string Name { get; }
    int Priority { get; }
    Task HandleAsync(object message);
    bool ShouldHandle(object message);
}

/// <summary>
/// 消息处理器包装器
/// </summary>
/// <typeparam name="TMessage">消息类型</typeparam>
internal class MessageHandlerWrapper<TMessage> : IMessageHandler where TMessage : class
{
    private readonly IMessageHandler<TMessage> _handler;

    public MessageHandlerWrapper(IMessageHandler<TMessage> handler)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
    }

    public string Name => _handler.Name;
    public int Priority => _handler.Priority;

    public async Task HandleAsync(object message)
    {
        if (message is TMessage typedMessage)
        {
            await _handler.HandleAsync(typedMessage);
        }
    }

    public bool ShouldHandle(object message)
    {
        return message is TMessage typedMessage && _handler.ShouldHandle(typedMessage);
    }
}

/// <summary>
/// 消息处理器包装器（带结果）
/// </summary>
/// <typeparam name="TMessage">消息类型</typeparam>
/// <typeparam name="TResult">结果类型</typeparam>
internal class MessageHandlerWrapper<TMessage, TResult> : IMessageHandler where TMessage : class
{
    private readonly IMessageHandler<TMessage, TResult> _handler;

    public MessageHandlerWrapper(IMessageHandler<TMessage, TResult> handler)
    {
        _handler = handler ?? throw new ArgumentNullException(nameof(handler));
    }

    public string Name => _handler.Name;
    public int Priority => _handler.Priority;

    public async Task HandleAsync(object message)
    {
        if (message is TMessage typedMessage)
        {
            await _handler.HandleAsync(typedMessage);
        }
    }

    public bool ShouldHandle(object message)
    {
        return message is TMessage typedMessage && _handler.ShouldHandle(typedMessage);
    }
}
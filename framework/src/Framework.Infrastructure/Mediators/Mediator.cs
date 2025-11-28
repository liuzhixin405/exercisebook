using System.Collections.Concurrent;

namespace Framework.Infrastructure.Mediators;

/// <summary>
/// 中介者实现 - 中介者模式
/// 提供对象间通信的实现
/// </summary>
public class Mediator : IMediator
{
    private readonly ConcurrentDictionary<Type, IInternalMessageHandler> _handlers;

    /// <summary>
    /// 构造函数
    /// </summary>
    public Mediator()
    {
        _handlers = new ConcurrentDictionary<Type, IInternalMessageHandler>();
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

        if (handler.ShouldHandle(message))
        {
            await handler.HandleAsync(message);
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

        if (handler.ShouldHandle(message))
        {
            var resultObj = await handler.HandleAsync(message);
            if (resultObj is TResult result)
                return result;
            return default!;
        }

        throw new InvalidOperationException($"Handler for message type {messageType.Name} does not support result type {typeof(TResult).Name}");
    }

    /// <inheritdoc />
    public IMediator RegisterHandler<TMessage>(IMessageHandler<TMessage> handler) where TMessage : class
    {
        if (handler == null)
            throw new ArgumentNullException(nameof(handler));

        var messageType = typeof(TMessage);
        var adapter = new MessageHandlerAdapter<TMessage>(handler);
        _handlers.AddOrUpdate(messageType, adapter, (key, existing) => adapter);
        return this;
    }

    /// <inheritdoc />
    public IMediator RegisterHandler<TMessage, TResult>(IMessageHandler<TMessage, TResult> handler) where TMessage : class
    {
        if (handler == null)
            throw new ArgumentNullException(nameof(handler));

        var messageType = typeof(TMessage);
        var adapter = new MessageHandlerWithResultAdapter<TMessage, TResult>(handler);
        _handlers.AddOrUpdate(messageType, adapter, (key, existing) => adapter);
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

internal interface IInternalMessageHandler
{
    Task<object?> HandleAsync(object message);
    bool ShouldHandle(object message);
}

internal class MessageHandlerAdapter<TMessage> : IInternalMessageHandler where TMessage : class
{
    private readonly IMessageHandler<TMessage> _inner;

    public MessageHandlerAdapter(IMessageHandler<TMessage> inner)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
    }

    public async Task<object?> HandleAsync(object message)
    {
        if (message is TMessage typed)
        {
            await _inner.HandleAsync(typed);
            return null;
        }
        throw new InvalidOperationException($"Message type mismatch. Expected {typeof(TMessage).Name}");
    }

    public bool ShouldHandle(object message)
    {
        if (message is TMessage typed)
        {
            return _inner.ShouldHandle(typed);
        }
        return false;
    }
}

internal class MessageHandlerWithResultAdapter<TMessage, TResult> : IInternalMessageHandler where TMessage : class
{
    private readonly IMessageHandler<TMessage, TResult> _inner;

    public MessageHandlerWithResultAdapter(IMessageHandler<TMessage, TResult> inner)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
    }

    public async Task<object?> HandleAsync(object message)
    {
        if (message is TMessage typed)
        {
            var result = await _inner.HandleAsync(typed);
            return (object?)result;
        }
        throw new InvalidOperationException($"Message type mismatch. Expected {typeof(TMessage).Name}");
    }

    public bool ShouldHandle(object message)
    {
        if (message is TMessage typed)
        {
            return _inner.ShouldHandle(typed);
        }
        return false;
    }
}

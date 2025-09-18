namespace Framework.Infrastructure.Mediators;

/// <summary>
/// 中介者接口 - 中介者模式
/// 提供对象间通信的抽象
/// </summary>
public interface IMediator
{
    /// <summary>
    /// 发送消息
    /// </summary>
    /// <typeparam name="TMessage">消息类型</typeparam>
    /// <param name="message">消息</param>
    /// <returns>任务</returns>
    Task SendAsync<TMessage>(TMessage message) where TMessage : class;

    /// <summary>
    /// 发送消息并返回结果
    /// </summary>
    /// <typeparam name="TMessage">消息类型</typeparam>
    /// <typeparam name="TResult">结果类型</typeparam>
    /// <param name="message">消息</param>
    /// <returns>结果</returns>
    Task<TResult> SendAsync<TMessage, TResult>(TMessage message) where TMessage : class;

    /// <summary>
    /// 注册处理器
    /// </summary>
    /// <typeparam name="TMessage">消息类型</typeparam>
    /// <param name="handler">处理器</param>
    /// <returns>中介者</returns>
    IMediator RegisterHandler<TMessage>(IMessageHandler<TMessage> handler) where TMessage : class;

    /// <summary>
    /// 注册处理器（带结果）
    /// </summary>
    /// <typeparam name="TMessage">消息类型</typeparam>
    /// <typeparam name="TResult">结果类型</typeparam>
    /// <param name="handler">处理器</param>
    /// <returns>中介者</returns>
    IMediator RegisterHandler<TMessage, TResult>(IMessageHandler<TMessage, TResult> handler) where TMessage : class;

    /// <summary>
    /// 取消注册处理器
    /// </summary>
    /// <typeparam name="TMessage">消息类型</typeparam>
    /// <returns>中介者</returns>
    IMediator UnregisterHandler<TMessage>() where TMessage : class;
}

/// <summary>
/// 消息处理器接口
/// </summary>
/// <typeparam name="TMessage">消息类型</typeparam>
public interface IMessageHandler<in TMessage> where TMessage : class
{
    /// <summary>
    /// 处理器名称
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 处理器优先级
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// 处理消息
    /// </summary>
    /// <param name="message">消息</param>
    /// <returns>任务</returns>
    Task HandleAsync(TMessage message);

    /// <summary>
    /// 是否应该处理此消息
    /// </summary>
    /// <param name="message">消息</param>
    /// <returns>是否应该处理</returns>
    bool ShouldHandle(TMessage message);
}

/// <summary>
/// 带结果的消息处理器接口
/// </summary>
/// <typeparam name="TMessage">消息类型</typeparam>
/// <typeparam name="TResult">结果类型</typeparam>
public interface IMessageHandler<in TMessage, TResult> where TMessage : class
{
    /// <summary>
    /// 处理器名称
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 处理器优先级
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// 处理消息并返回结果
    /// </summary>
    /// <param name="message">消息</param>
    /// <returns>结果</returns>
    Task<TResult> HandleAsync(TMessage message);

    /// <summary>
    /// 是否应该处理此消息
    /// </summary>
    /// <param name="message">消息</param>
    /// <returns>是否应该处理</returns>
    bool ShouldHandle(TMessage message);
}

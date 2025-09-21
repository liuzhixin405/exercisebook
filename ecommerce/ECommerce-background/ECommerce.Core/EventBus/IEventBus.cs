namespace ECommerce.Core.EventBus
{
    /// <summary>
    /// 分布式事件总线接口
    /// </summary>
    public interface IEventBus
    {
        /// <summary>
        /// 发布事件到消息队列
        /// </summary>
        Task PublishAsync<T>(T @event) where T : class;
        
        /// <summary>
        /// 订阅事件类型
        /// </summary>
        void Subscribe<T, TH>() 
            where T : class 
            where TH : IEventHandler<T>;
        
        /// <summary>
        /// 取消订阅
        /// </summary>
        void Unsubscribe<T, TH>() 
            where T : class 
            where TH : IEventHandler<T>;
    }

    /// <summary>
    /// 事件处理器接口
    /// </summary>
    public interface IEventHandler<in T> where T : class
    {
        Task<bool> HandleAsync(T @event, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// 事件订阅配置
    /// </summary>
    public class EventSubscription
    {
        public Type EventType { get; set; } = null!;
        public Type HandlerType { get; set; } = null!;
        public string EventName { get; set; } = string.Empty;
        public string HandlerName { get; set; } = string.Empty;
    }
}

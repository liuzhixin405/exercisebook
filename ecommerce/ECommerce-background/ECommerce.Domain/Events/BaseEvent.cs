namespace ECommerce.Domain.Events
{
    public abstract class BaseEvent
    {
        public Guid EventId { get; set; }
        public DateTime OccurredOn { get; set; }
        public string EventType { get; set; }
        public string CorrelationId { get; set; } = string.Empty; // 用于追踪业务流程
        public string Source { get; set; } = string.Empty; // 事件来源
        public Dictionary<string, object> Metadata { get; set; } = new(); // 扩展元数据
        
        protected BaseEvent()
        {
            EventId = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
            EventType = GetType().Name;
        }
    }
}
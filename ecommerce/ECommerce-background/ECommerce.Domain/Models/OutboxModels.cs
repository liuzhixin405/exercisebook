using ECommerce.Domain.Events;

namespace ECommerce.Domain.Models
{
    /// <summary>
    /// Outbox消息模型
    /// </summary>
    public class OutboxMessage
    {
        public Guid Id { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string EventData { get; set; } = string.Empty;
        public OutboxMessageStatus Status { get; set; }
        public int RetryCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public string? ErrorMessage { get; set; }
    }

    /// <summary>
    /// Outbox消息状态
    /// </summary>
    public enum OutboxMessageStatus
    {
        Pending,
        Processing,
        Completed,
        Failed
    }

    /// <summary>
    /// 基础领域事件
    /// </summary>
    public abstract class DomainEvent
    {
        public Guid Id { get; set; }
        public DateTime OccurredOn { get; set; }
        public string EventType { get; set; } = string.Empty;
        
        protected DomainEvent()
        {
            Id = Guid.NewGuid();
            OccurredOn = DateTime.UtcNow;
            EventType = GetType().Name;
        }
        
        public abstract string Data { get; }
    }


}

using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Events
{
    public class OrderStatusChangedEvent : BaseEvent
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public OrderStatus OldStatus { get; set; }
        public OrderStatus NewStatus { get; set; }
        public DateTime ChangedAt { get; set; }
        public string? Reason { get; set; } // 状态变更原因
        public string? Operator { get; set; } // 操作人
        public Dictionary<string, object> StatusData { get; set; } = new(); // 状态相关的数据

        public OrderStatusChangedEvent(Guid orderId, Guid userId, OrderStatus oldStatus, OrderStatus newStatus, string? reason = null, string? operatorName = null)
        {
            OrderId = orderId;
            UserId = userId;
            OldStatus = oldStatus;
            NewStatus = newStatus;
            ChangedAt = DateTime.UtcNow;
            Reason = reason;
            Operator = operatorName;
            CorrelationId = orderId.ToString();
            Source = "OrderService";
            
            // 根据状态设置相关数据
            if (newStatus == OrderStatus.Shipped)
            {
                StatusData["ShippedAt"] = ChangedAt;
            }
            else if (newStatus == OrderStatus.Delivered)
            {
                StatusData["DeliveredAt"] = ChangedAt;
            }
            else if (newStatus == OrderStatus.Cancelled)
            {
                StatusData["CancelledAt"] = ChangedAt;
            }
        }
    }
}

using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Events
{
    public class OrderCancelledEvent : BaseEvent
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? CancelledBy { get; set; } // 取消人
        public DateTime CancelledAt { get; set; }
        public OrderStatus PreviousStatus { get; set; }
        public bool RequiresRefund { get; set; } // 是否需要退款
        public decimal? RefundAmount { get; set; } // 退款金额

        public OrderCancelledEvent(Guid orderId, Guid userId, string reason, OrderStatus previousStatus, string? cancelledBy = null, bool requiresRefund = false, decimal? refundAmount = null)
        {
            OrderId = orderId;
            UserId = userId;
            Reason = reason;
            CancelledBy = cancelledBy;
            CancelledAt = DateTime.UtcNow;
            PreviousStatus = previousStatus;
            RequiresRefund = requiresRefund;
            RefundAmount = refundAmount;
            CorrelationId = orderId.ToString();
            Source = "OrderService";
        }
    }
}

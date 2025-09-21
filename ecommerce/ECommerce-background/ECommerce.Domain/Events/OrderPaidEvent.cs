using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Events
{
    public class OrderPaidEvent : BaseEvent
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public string PaymentId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
        public string Currency { get; set; } = "CNY";
        public Dictionary<string, string> PaymentMetadata { get; set; } = new();

        public OrderPaidEvent(Guid orderId, Guid userId, string paymentId, decimal amount, string paymentMethod, Dictionary<string, string>? metadata = null)
        {
            OrderId = orderId;
            UserId = userId;
            PaymentId = paymentId;
            Amount = amount;
            PaymentMethod = paymentMethod;
            PaymentDate = DateTime.UtcNow;
            CorrelationId = orderId.ToString();
            Source = "PaymentService";
            
            if (metadata != null)
            {
                PaymentMetadata = metadata;
            }
        }
    }
}

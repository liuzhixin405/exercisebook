using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Events
{
    public class PaymentSucceededEvent : BaseEvent
    {
        public string PaymentId { get; set; } = string.Empty;
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Currency { get; set; } = "CNY";
        public Dictionary<string, string> PaymentMetadata { get; set; } = new();

        public PaymentSucceededEvent(string paymentId, Guid orderId, Guid userId, decimal amount, string paymentMethod, Dictionary<string, string>? metadata = null)
        {
            PaymentId = paymentId;
            OrderId = orderId;
            UserId = userId;
            Amount = amount;
            PaymentMethod = paymentMethod;
            CorrelationId = orderId.ToString();
            Source = "PaymentService";

            if (metadata != null)
            {
                PaymentMetadata = metadata;
            }
        }
    }
}



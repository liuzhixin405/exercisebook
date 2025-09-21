using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Events
{
    public class PaymentFailedEvent : BaseEvent
    {
        public string PaymentId { get; set; } = string.Empty;
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string? ErrorMessage { get; set; }
        public string Currency { get; set; } = "CNY";
        public Dictionary<string, string> PaymentMetadata { get; set; } = new();

        public PaymentFailedEvent(string paymentId, Guid orderId, Guid userId, decimal amount, string paymentMethod, string? errorMessage = null, Dictionary<string, string>? metadata = null)
        {
            PaymentId = paymentId;
            OrderId = orderId;
            UserId = userId;
            Amount = amount;
            PaymentMethod = paymentMethod;
            ErrorMessage = errorMessage;
            CorrelationId = orderId.ToString();
            Source = "PaymentService";

            if (metadata != null)
            {
                PaymentMetadata = metadata;
            }
        }
    }
}



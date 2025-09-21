using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Events
{
    public class PaymentProcessedEvent : BaseEvent
    {
        public string PaymentId { get; set; } = string.Empty;
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime ProcessedAt { get; set; }
        public string Currency { get; set; } = "CNY";
        public string? TransactionId { get; set; } // 第三方支付平台的交易ID
        public Dictionary<string, string> PaymentMetadata { get; set; } = new();

        public PaymentProcessedEvent(string paymentId, Guid orderId, Guid userId, decimal amount, string paymentMethod, bool success, string? errorMessage = null, string? transactionId = null, Dictionary<string, string>? metadata = null)
        {
            PaymentId = paymentId;
            OrderId = orderId;
            UserId = userId;
            Amount = amount;
            PaymentMethod = paymentMethod;
            Success = success;
            ErrorMessage = errorMessage;
            TransactionId = transactionId;
            ProcessedAt = DateTime.UtcNow;
            CorrelationId = orderId.ToString();
            Source = "PaymentService";
            
            if (metadata != null)
            {
                PaymentMetadata = metadata;
            }
        }
    }
}

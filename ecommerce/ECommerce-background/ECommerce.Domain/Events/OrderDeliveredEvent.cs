using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Events
{
    public class OrderDeliveredEvent : BaseEvent
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public DateTime DeliveredAt { get; set; }
        public string? DeliveryNotes { get; set; } // 送达备注
        public string? RecipientSignature { get; set; } // 收件人签名
        public bool IsOnTime { get; set; } // 是否按时送达
        public Dictionary<string, object> DeliveryData { get; set; } = new(); // 送达相关数据

        public OrderDeliveredEvent(Guid orderId, Guid userId, string? deliveryNotes = null, string? recipientSignature = null, bool isOnTime = true)
        {
            OrderId = orderId;
            UserId = userId;
            DeliveredAt = DateTime.UtcNow;
            DeliveryNotes = deliveryNotes;
            RecipientSignature = recipientSignature;
            IsOnTime = isOnTime;
            CorrelationId = orderId.ToString();
            Source = "OrderService";
            
            // 设置送达相关数据
            DeliveryData["DeliveredAt"] = DeliveredAt;
            DeliveryData["IsOnTime"] = isOnTime;
            if (!string.IsNullOrEmpty(deliveryNotes))
            {
                DeliveryData["DeliveryNotes"] = deliveryNotes;
            }
            if (!string.IsNullOrEmpty(recipientSignature))
            {
                DeliveryData["RecipientSignature"] = recipientSignature;
            }
        }
    }
}

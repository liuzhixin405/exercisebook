using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Events
{
    public class OrderShippedEvent : BaseEvent
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public string TrackingNumber { get; set; } = string.Empty;
        public string ShippingCompany { get; set; } = string.Empty; // 物流公司
        public DateTime ShippedAt { get; set; }
        public string? EstimatedDeliveryDate { get; set; } // 预计送达日期
        public string? ShippingNotes { get; set; } // 发货备注
        public Dictionary<string, object> ShippingData { get; set; } = new(); // 发货相关数据

        public OrderShippedEvent(Guid orderId, Guid userId, string trackingNumber, string shippingCompany, string? estimatedDeliveryDate = null, string? shippingNotes = null)
        {
            OrderId = orderId;
            UserId = userId;
            TrackingNumber = trackingNumber;
            ShippingCompany = shippingCompany;
            ShippedAt = DateTime.UtcNow;
            EstimatedDeliveryDate = estimatedDeliveryDate;
            ShippingNotes = shippingNotes;
            CorrelationId = orderId.ToString();
            Source = "OrderService";
            
            // 设置发货相关数据
            ShippingData["TrackingNumber"] = trackingNumber;
            ShippingData["ShippingCompany"] = shippingCompany;
            ShippingData["ShippedAt"] = ShippedAt;
            if (!string.IsNullOrEmpty(estimatedDeliveryDate))
            {
                ShippingData["EstimatedDeliveryDate"] = estimatedDeliveryDate;
            }
        }
    }
}

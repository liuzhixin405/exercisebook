using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Events
{
    public class StockLockedEvent : BaseEvent
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public Guid OrderId { get; set; }
        public DateTime LockedAt { get; set; }

        public StockLockedEvent(Guid productId, string productName, int quantity, Guid orderId)
        {
            ProductId = productId;
            ProductName = productName;
            Quantity = quantity;
            OrderId = orderId;
            LockedAt = DateTime.UtcNow;
            CorrelationId = orderId.ToString();
            Source = "InventoryService";
        }
    }
}

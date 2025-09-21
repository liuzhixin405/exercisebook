using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Events
{
    public class OrderCreatedEvent : BaseEvent
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public List<OrderItemInfo> Items { get; set; } = new List<OrderItemInfo>();

        public OrderCreatedEvent(Order order)
        {
            OrderId = order.Id;
            UserId = order.UserId;
            CustomerName = order.CustomerName;
            TotalAmount = order.TotalAmount;
            CreatedAt = order.CreatedAt;
            PaymentMethod = order.PaymentMethod;
            ShippingAddress = order.ShippingAddress;
            CorrelationId = order.Id.ToString(); // 使用订单ID作为关联ID
            Source = "OrderService";
            Items = order.Items.Select(item => new OrderItemInfo
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = item.Price
            }).ToList();
        }
    }

    public class OrderItemInfo
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
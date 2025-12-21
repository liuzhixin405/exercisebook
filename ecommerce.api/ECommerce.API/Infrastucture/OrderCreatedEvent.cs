using ECommerce.API.Application;

namespace ECommerce.API.Infrastucture
{
    // 订单创建事件
    public class OrderCreatedEvent : IDomainEvent
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public DateTime OccurredOn { get; set; } = DateTime.UtcNow;
    }
}

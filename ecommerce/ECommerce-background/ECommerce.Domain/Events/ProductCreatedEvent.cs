using ECommerce.Domain.Entities;

namespace ECommerce.Domain.Events
{
    public class ProductCreatedEvent : BaseEvent
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        
        public ProductCreatedEvent(Guid productId, string name, decimal price, int stock)
        {
            ProductId = productId;
            Name = name;
            Price = price;
            Stock = stock;
        }
    }
}
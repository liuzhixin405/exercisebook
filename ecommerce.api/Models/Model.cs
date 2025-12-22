using ECommerce.API.Infrastructure;

namespace ECommerce.API.Models
{
    public class OrderRequest
    {
        public string CustomerId { get; set; }
        public decimal Amount { get; set; }
        public string PaymentType { get; set; }
        public List<OrderItem> Items { get; set; }
    }

    public class Order
    {
        public int Id { get; set; }
        public string CustomerId { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PaymentResult
    {
        public bool Success { get; set; }
        public Guid TransactionId { get; set; }
        public string ErrorMessage { get; set; }
    }

    public interface IOrderRepository
    {
        Task<Order> GetByIdAsync(int id);
        Task AddAsync(Order order);
    }

    public class OrderRepository : IOrderRepository
    {
        // 实现略
        public Task AddAsync(Order order)
        {
            throw new NotImplementedException();
        }

        public Task<Order> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }

}

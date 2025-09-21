using System.ComponentModel.DataAnnotations;

namespace ECommerce.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        
        [Required]
        public Guid UserId { get; set; }
        
        public virtual ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        
        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }
        
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        
        [MaxLength(200)]
        public string ShippingAddress { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string CustomerName { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PaidAt { get; set; }
        public DateTime? ShippedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        
        [MaxLength(100)]
        public string PaymentMethod { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string TrackingNumber { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string Notes { get; set; } = string.Empty;
        
        // Navigation properties
        public virtual Models.User User { get; set; } = null!;
    }
    
    public class OrderItem
    {
        public Guid Id { get; set; }
        
        [Required]
        public Guid OrderId { get; set; }
        
        [Required]
        public Guid ProductId { get; set; }
        
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
        
        // Navigation properties
        public virtual Order Order { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
    
    public enum OrderStatus
    {
        Pending,        // 待支付
        Paid,           // 已支付
        Confirmed,      // 已确认
        Shipped,        // 已发货
        Delivered,      // 已送达
        Completed,      // 已完成
        Cancelled,      // 已取消
        Refunded        // 已退款
    }
}
using ECommerce.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Domain.Models
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [Required]
        [StringLength(200)]
        public string ShippingAddress { get; set; } = string.Empty;
        
        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }
        
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime? ShippedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        
        [StringLength(100)]
        public string PaymentMethod { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string TrackingNumber { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;
        
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
    }

    public class OrderItemDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        
        [StringLength(200)]
        public string ProductName { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string ProductImage { get; set; } = string.Empty;
        
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal Subtotal { get; set; }
    }

    public class CreateOrderDto
    {
        public Guid? AddressId { get; set; }
        
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string ShippingAddress { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string PaymentMethod { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;
        
        [MinLength(1)]
        public List<CreateOrderItemDto> Items { get; set; } = new List<CreateOrderItemDto>();
    }

    public class CreateOrderItemDto
    {
        [Required]
        public Guid ProductId { get; set; }
        
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }

    public class UpdateOrderStatusDto
    {
        [Required]
        public OrderStatus Status { get; set; }
        
        [StringLength(100)]
        public string TrackingNumber { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;
    }

    public class PaymentDto
    {
        [Required]
        public Guid OrderId { get; set; }
        
        [StringLength(100)]
        public string PaymentMethod { get; set; } = string.Empty;
        
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }
    }
}

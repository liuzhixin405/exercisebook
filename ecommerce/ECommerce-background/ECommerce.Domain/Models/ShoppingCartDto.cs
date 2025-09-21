using System.ComponentModel.DataAnnotations;

namespace ECommerce.Domain.Models
{
    /// <summary>
    /// 购物车DTO
    /// </summary>
    public class ShoppingCartDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<ShoppingCartItemDto> Items { get; set; } = new List<ShoppingCartItemDto>();
        public int TotalItems { get; set; }
        public decimal TotalAmount { get; set; }
    }

    /// <summary>
    /// 购物车项DTO
    /// </summary>
    public class ShoppingCartItemDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        
        [Required]
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

    /// <summary>
    /// 添加到购物车DTO
    /// </summary>
    public class AddToCartDto
    {
        [Required]
        public Guid ProductId { get; set; }
        
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; } = 1;
    }

    /// <summary>
    /// 更新购物车项DTO
    /// </summary>
    public class UpdateCartItemDto
    {
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
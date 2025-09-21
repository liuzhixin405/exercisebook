using ECommerce.Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Domain.Entities
{
    /// <summary>
    /// 购物车
    /// </summary>
    public class ShoppingCart
    {
        public Guid Id { get; set; }
        
        [Required]
        public Guid UserId { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual ICollection<ShoppingCartItem> Items { get; set; } = new List<ShoppingCartItem>();
        public virtual User User { get; set; } = null!;
    }
    
    /// <summary>
    /// 购物车项
    /// </summary>
    public class ShoppingCartItem
    {
        public Guid Id { get; set; }
        
        [Required]
        public Guid ShoppingCartId { get; set; }
        
        [Required]
        public Guid ProductId { get; set; }
        
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual ShoppingCart ShoppingCart { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}
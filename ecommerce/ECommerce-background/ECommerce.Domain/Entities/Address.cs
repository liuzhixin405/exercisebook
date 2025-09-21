using System.ComponentModel.DataAnnotations;

namespace ECommerce.Domain.Entities
{
    public class Address
    {
        public Guid Id { get; set; }
        
        [Required]
        public Guid UserId { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty; // 收货人姓名
        
        [Required]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty; // 收货人电话
        
        [Required]
        [MaxLength(200)]
        public string Province { get; set; } = string.Empty; // 省份
        
        [Required]
        [MaxLength(200)]
        public string City { get; set; } = string.Empty; // 城市
        
        [Required]
        [MaxLength(200)]
        public string District { get; set; } = string.Empty; // 区县
        
        [Required]
        [MaxLength(500)]
        public string Street { get; set; } = string.Empty; // 街道地址
        
        [MaxLength(10)]
        public string PostalCode { get; set; } = string.Empty; // 邮政编码
        
        public bool IsDefault { get; set; } = false; // 是否为默认地址
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual Models.User User { get; set; } = null!;
        
        // 计算属性：完整地址
        public string FullAddress => $"{Province} {City} {District} {Street}";
    }
}

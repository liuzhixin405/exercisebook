using System.ComponentModel.DataAnnotations;

namespace Pandora.Cigfi.Models.Cigfi
{
    /// <summary>
    /// 商品分类表，对应数据库表 cigfi_category
    /// </summary>
    public class Category
    {
        [Key]
        public long Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public long? ParentId { get; set; }

        public int SortOrder { get; set; }
    }
}
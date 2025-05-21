using System;
using System.ComponentModel.DataAnnotations;

namespace Pandora.Cigfi.Models.Cigfi
{
    /// <summary>
    /// 首页轮播图表，对应数据库表 cigfi_banner
    /// </summary>
    public class Banner
    {
        [Key]
        public long Id { get; set; }

        [MaxLength(500)]
        public string ImageUrl { get; set; }

        [MaxLength(500)]
        public string LinkUrl { get; set; }

        public int SortOrder { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
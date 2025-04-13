using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace spot.Domain.Products.Entities
{
    public class Product
    {
        [Key]
        [Column("id")]
        public string Id { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Column("createdAt")]
        public long CreatedAt { get; set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        [Column("updatedAt")]
        public long UpdatedAt { get; set; }
        /// <summary>
        /// 基础币种
        /// </summary>

        [Column("base_currency")]
        public string BaseCurrency { get; set; }
        /// <summary>
        /// 报价币种
        /// </summary>

        [Column("quote_currency")]
        public string QuoteCurrency { get; set; }

        // Using decimal for high precision decimal values
        [Column("base_min_size")]
        public decimal BaseMinSize { get; set; }

        [Column("base_max_size")]
        public decimal BaseMaxSize { get; set; }

        [Column("quote_min_size")]
        public decimal QuoteMinSize { get; set; }

        [Column("quote_max_size")]
        public decimal QuoteMaxSize { get; set; }

        // Integer type for scale (32-bit in Go, corresponds to int in C#)
        [Column("base_scale")]
        public int BaseScale { get; set; }

        [Column("quote_scale")]
        public int QuoteScale { get; set; }

        [Column("quote_increment")]
        public double QuoteIncrement { get; set; }
    }
}

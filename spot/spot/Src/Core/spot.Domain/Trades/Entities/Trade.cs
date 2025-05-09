using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace spot.Domain.Trades.Entities
{
    public class Trade
    {
        [Key]
        [Column("id")]
        public required string Id { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Column("created_at")]
        public long CreatedAt { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        [Column("product_id")]
        public required string ProductId { get; set; }

        /// <summary>
        /// 交易价格
        /// </summary>
        [Column("price")]
        public decimal Price { get; set; }

        /// <summary>
        /// 交易数量
        /// </summary>
        [Column("size")]
        public decimal Size { get; set; }

        /// <summary>
        /// 买方订单ID
        /// </summary>
        [Column("taker_order_id")]
        public required string TakerOrderId { get; set; }

        /// <summary>
        /// 卖方订单ID
        /// </summary>
        [Column("maker_order_id")]
        public required string MakerOrderId { get; set; }

        /// <summary>
        /// 交易方向
        /// </summary>
        [Column("side")]
        public required string Side { get; set; }

        /// <summary>
        /// 交易费用
        /// </summary>
        [Column("fee")]
        public decimal Fee { get; set; }

        /// <summary>
        /// 交易总额
        /// </summary>
        [NotMapped]
        public decimal Total => Price * Size;
    }
}
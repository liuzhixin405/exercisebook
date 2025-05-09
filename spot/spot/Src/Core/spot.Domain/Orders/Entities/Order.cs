using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace spot.Domain.Orders.Entities
{
    public class Order
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
        /// 更新时间
        /// </summary>
        [Column("updated_at")]
        public long UpdatedAt { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        [Column("product_id")]
        public required string ProductId { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [Column("user_id")]
        public required string UserId { get; set; }

        /// <summary>
        /// 订单类型（限价单/市价单）
        /// </summary>
        [Column("type")]
        public required string Type { get; set; }

        /// <summary>
        /// 订单方向（买/卖）
        /// </summary>
        [Column("side")]
        public required string Side { get; set; }

        /// <summary>
        /// 下单价格
        /// </summary>
        [Column("price")]
        public decimal Price { get; set; }

        /// <summary>
        /// 下单数量
        /// </summary>
        [Column("size")]
        public decimal Size { get; set; }

        /// <summary>
        /// 已成交数量
        /// </summary>
        [Column("filled_size")]
        public decimal FilledSize { get; set; }

        /// <summary>
        /// 已成交金额
        /// </summary>
        [Column("filled_funds")]
        public decimal FilledFunds { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        [Column("status")]
        public required string Status { get; set; }

        /// <summary>
        /// 是否完全成交
        /// </summary>
        [NotMapped]
        public bool IsFullyFilled => FilledSize >= Size;
    }
}
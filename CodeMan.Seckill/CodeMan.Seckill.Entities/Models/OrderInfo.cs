using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeMan.Seckill.Entities.Models
{
    [Table("order_info")]
    public class OrderInfo
    {
        [Key]
        [Column("order_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }
        [Column("user_id")]
        public int UserId { get; set; }
        [Column("goods_id")]
        public int GoodsId { get; set; }
        [Column("goods_name")]
        public string GoodsName { get; set; }
        [Column("goods_count")]
        public int GoodsCount { get; set; }
        [Column("goods_price")]
        public double GoodsPrice { get; set; }
        public int Status { get; set; }
        [Column("create_time")]
        public DateTime CreateTime { get; set; } = DateTime.Now;


    }
}
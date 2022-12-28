using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeMan.Seckill.Entities.Models
{
    [Table("seckill_goods")]
    public class SeckillGoods
    {
        [Key]
        [Column("seckill_goods_id")]
        public int SeckillGoodsId { get; set; }
        [Column("goods_id")]
        public int GoodsId { get; set; }
        [Column("stock_count")]
        public int StockCount { get; set; }
        [Column("start_time")]
        public DateTime StartTime { get; set; }
        [Column("end_time")]
        public DateTime EndTime { get; set; }
    }
}
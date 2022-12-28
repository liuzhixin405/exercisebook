using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeMan.Seckill.Entities.Models
{
    [Table("goods")]
    public class Goods
    {
        [Key]
        [Column("goods_id")]
        public int GoodsId { get; set; }
        [Column("goods_name")]
        public string GoodsName { get; set; }
        [Column("goods_stock")]
        public int GoodsStock { get; set; }
    }
}
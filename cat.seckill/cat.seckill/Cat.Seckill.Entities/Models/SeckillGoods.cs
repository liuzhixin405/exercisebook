using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cat.Seckill.Entities.Models
{
    public class SeckillGoods
    {
        public int Id { get; set; }
        public int GoodsId { get; set; }
        public int StockCount { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}

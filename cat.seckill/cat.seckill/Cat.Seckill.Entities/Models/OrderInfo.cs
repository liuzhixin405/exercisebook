using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cat.Seckill.Entities.Models
{
    public class OrderInfo
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int GoodsId { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
        public int Status { get; set; }
        public DateTime Creattime { get; set; } = DateTime.Now;

    }
}

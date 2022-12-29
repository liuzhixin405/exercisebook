using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cat.Seckill.Base.RabbitMq.Message
{
    public class SeckillMessage
    {
        public int UserId { get; set; }
        public int GoodsId { get; set; }
    }
}

using Cat.Seckill.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cat.Seckill.Base.RabbitMq.Message
{
    public class OrderMessage
    {
        public OrderInfo OrderInfo { get; set; }
        public Account Account { get; set; }
    }
}

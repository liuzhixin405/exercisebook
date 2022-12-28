using CodeMan.Seckill.Entities.Models;

namespace CodeMan.Seckill.Base.RabbitMq.Message
{
    public class OrderMessage
    {
        public OrderInfo OrderInfo { get; set; }
        public Account Account { get; set; }
    }
}
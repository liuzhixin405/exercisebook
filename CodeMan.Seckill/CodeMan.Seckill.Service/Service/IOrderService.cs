using CodeMan.Seckill.Base.RabbitMq.Message;
using CodeMan.Seckill.Entities.Models;

namespace CodeMan.Seckill.Service.service
{
    public interface IOrderService
    {
        void Update(int userId, int goodsId);
        OrderInfo CreateOrder(SeckillMessage seckillMessage);

        void UpdatePayState(OrderInfo orderInfo);
    }
}
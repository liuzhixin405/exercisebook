using CodeMan.Seckill.Base.RabbitMq.Message;
using CodeMan.Seckill.Entities.Models;

namespace CodeMan.Seckill.Service.service
{
    public interface ISeckillService
    {
        void Seckill(SeckillMessage seckillMessage);
    }
}
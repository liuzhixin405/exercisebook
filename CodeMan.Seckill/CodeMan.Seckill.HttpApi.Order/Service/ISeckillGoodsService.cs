using System.Collections.Generic;
using CodeMan.Seckill.Base.RabbitMq.Message;
using CodeMan.Seckill.Entities.Models;

namespace CodeMan.Seckill.HttpApi.Order.service
{
    public interface ISeckillGoodsService
    {
        public IEnumerable<SeckillGoods> Reset();
        void SendSeckillMessage(SeckillMessage seckillMessage);
        void SendMessage(string content);
        void ResetStock(int goodsId);
    }
}
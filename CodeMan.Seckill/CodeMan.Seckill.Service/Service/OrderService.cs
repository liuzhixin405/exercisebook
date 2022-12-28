using System;
using System.Collections.Generic;
using CodeMan.Seckill.Base.RabbitMq;
using CodeMan.Seckill.Base.RabbitMq.Message;
using CodeMan.Seckill.Base.Redis;
using CodeMan.Seckill.Entities.Models;
using CodeMan.Seckill.Service.Repository;
using Newtonsoft.Json;

namespace CodeMan.Seckill.Service.service
{
    public class OrderService : IOrderService
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IRabbitProducer _rabbitProducer;
        private readonly RedisHelper _redis;

        public OrderService(IRepositoryWrapper repositoryWrapper, IRabbitProducer rabbitProducer, RedisHelper redis)
        {
            _repositoryWrapper = repositoryWrapper;
            _rabbitProducer = rabbitProducer;
            _redis = redis;
        }

        public void Update(int userId, int goodsId)
        {
            SeckillGoods seckillGoods = _repositoryWrapper.SeckillGoods.GetSeckillGoodsByGoodsId(goodsId);
            seckillGoods.StockCount--;
            Console.WriteLine($"{userId}抢购成功了，商品是：{goodsId}");
            _repositoryWrapper.SeckillGoods.Update(seckillGoods);
            _repositoryWrapper.Save();
        }

        public OrderInfo CreateOrder(SeckillMessage seckillMessage)
        {
            Goods goods = _repositoryWrapper.Goods.GetGoodsById(seckillMessage.GoodsId);
            OrderInfo orderInfo = new OrderInfo();
            orderInfo.GoodsCount = 1;
            orderInfo.GoodsId = seckillMessage.GoodsId;
            orderInfo.GoodsName = goods.GoodsName;
            orderInfo.Status = 0;
            orderInfo.UserId = seckillMessage.UserId;
            _repositoryWrapper.OrderInfo.CreateOrder(orderInfo);
            _repositoryWrapper.Save();
            Console.WriteLine($"用户{seckillMessage.UserId}抢购成功了，商品是：{seckillMessage.GoodsId}");
            _redis.Set(RedisConstant.SECKILL_USER_GOODS + seckillMessage.UserId + "_" + seckillMessage.GoodsId,
                JsonConvert.SerializeObject(orderInfo));

            // 执行到这里，已经把订单写入到数据库，接下来异步通知
            Account account = _repositoryWrapper.User.GetUserById(seckillMessage.UserId);
            OrderMessage orderMessage = new OrderMessage();
            orderMessage.Account = account;
            orderMessage.OrderInfo = orderInfo;
            // // 邮件通知
            _rabbitProducer.Publish(RabbitConstant.EMAIL_EXCHANGE, "",
                new Dictionary<string, object>(),
                JsonConvert.SerializeObject(orderMessage));
            // // 短信通知
            _rabbitProducer.Publish(RabbitConstant.SMS_EXCHANGE, "",
                new Dictionary<string, object>(),
                JsonConvert.SerializeObject(orderMessage));
            // // 通过死信队列来处理超时未处理的订单，把状态由0改成-1
            _rabbitProducer.Publish(RabbitConstant.DELAY_EXCHANGE, RabbitConstant.DELAY_ROUTING_KEY,
                new Dictionary<string, object>()
                {
                    {"x-delay", 1000*20 }
                },
                JsonConvert.SerializeObject(orderMessage));
            return orderInfo;
        }

        public void UpdatePayState(OrderInfo orderInfo)
        {
            _repositoryWrapper.OrderInfo.Update(orderInfo);
        }
    }
}

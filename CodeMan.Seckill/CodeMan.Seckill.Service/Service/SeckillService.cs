using System;
using CodeMan.Seckill.Base.RabbitMq.Message;
using CodeMan.Seckill.Entities.Models;
using CodeMan.Seckill.Service.Repository;

namespace CodeMan.Seckill.Service.service
{
    public class SeckillService : ISeckillService
    {
        private readonly IGoodsService _goodsService;
        private readonly IOrderService _orderService;

        public SeckillService(IGoodsService goodsService, IOrderService orderService)
        {
            _goodsService = goodsService;
            _orderService = orderService;
        }
        public void Seckill(SeckillMessage seckillMessage)
        {
            bool result = _goodsService.ReduceStock(seckillMessage.GoodsId);
            if (result)
            {
                OrderInfo orderInfo = _orderService.CreateOrder(seckillMessage);
            }
            else
            {
                Console.WriteLine($"商品{seckillMessage.GoodsId}数据库库存为零，无法扣减库存");
            }
        }
    }
}
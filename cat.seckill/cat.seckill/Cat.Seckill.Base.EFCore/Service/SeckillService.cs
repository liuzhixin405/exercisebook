using Cat.Seckill.Base.RabbitMq;
using Cat.Seckill.Base.RabbitMq.Message;
using Cat.Seckill.Base.Redis;
using Cat.Seckill.Entities.BaseRepository;
using Cat.Seckill.Entities.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Cat.Seckill.Base.EFCore.Service
{
    public class SeckillService : ISeckillService
    {
        private readonly IGoodsService goodsService;
        private readonly IOrderService orderService;
        private readonly IRepository<SeckillGoods> repository;
        private readonly ILogger<SeckillService> logger;
        private readonly IUserService userService;
        //private readonly IRabbitProducer _rabbitProducer;
        private readonly IServiceScopeFactory serviceScopeFactory;
        public SeckillService(IGoodsService goodsService, IOrderService orderService, 
            IRepository<SeckillGoods> repository, 
            ILogger<SeckillService> logger,
            IUserService userService,
            //IRabbitProducer rabbitProducer,
            IServiceScopeFactory serviceScopeFactory)
        {
            this.goodsService = goodsService;
            this.orderService = orderService;
            this.repository = repository;
            this.logger = logger;
            this.serviceScopeFactory= serviceScopeFactory;
            this.userService= userService;
            //this._rabbitProducer= rabbitProducer;
        }

        public async Task<bool> Create(SeckillGoods seckillGoods)
        {
            var goods = await goodsService.FindById(seckillGoods.GoodsId);
            if (goods!=null)
            {
                if(goods.Stock < seckillGoods.StockCount)
                {
                    logger.LogInformation($"商品{seckillGoods.GoodsId}已售空");
                    return false;
                }
                else
                {
                    await repository.Create(seckillGoods);
                    return true;
                }
            }
            else
            {
                logger.LogInformation($"商品{seckillGoods.GoodsId}已售空");
                return false;
            }
           
        }
        
        public async Task<IEnumerable<SeckillGoods>> FindSeckillGoods()
        {
           return await repository.FindAll();
        }

        public async Task Seckill(int userId, int goodsId)
        {
            var body=System.Text.Json.JsonSerializer.Serialize(new SeckillMessage { GoodsId= goodsId , UserId= userId});
            logger.LogInformation($"send message:{body}");
            using var scope = serviceScopeFactory.CreateAsyncScope();
            var _rabbitProducer = scope.ServiceProvider.GetRequiredService<IRabbitProducer>();
            _rabbitProducer.Publish(RabbitConstant.SECKILL_EXCHANGE, "", new Dictionary<String, object>(), body);
          
        }

        public async Task SeckillOrder(SeckillMessage? seckillMessage)
        {
            bool result = await goodsService.ReduceStock(seckillMessage.GoodsId);
            if (result)
            {
                OrderInfo orderInfo = await orderService.Create(new OrderInfo
                {
                    Status = 0,
                    GoodsId = seckillMessage.GoodsId,
                    AccountId = seckillMessage.UserId,
                    Count = 1,
                    Creattime = DateTime.Now,
                    Price = 1
                });
                Console.WriteLine($"用户{seckillMessage.UserId}抢购成功了，商品是：{seckillMessage.GoodsId}");
                using var scope = serviceScopeFactory.CreateAsyncScope();
                var _redisHelper = scope.ServiceProvider.GetRequiredService<RedisHelper>();
                var _rabbitProducer = scope.ServiceProvider.GetRequiredService<IRabbitProducer>();
                _redisHelper.Set(RedisConstant.SECKILL_USER_GOODS + seckillMessage.UserId + "_" + seckillMessage.GoodsId,
                System.Text.Json.JsonSerializer.Serialize(orderInfo));

                var account =await userService.GetUserById(seckillMessage.UserId);
                OrderMessage orderMessage = new OrderMessage { Account=account, OrderInfo = orderInfo};
                // // 邮件通知
                _rabbitProducer.Publish(RabbitConstant.EMAIL_EXCHANGE, "",
                    new Dictionary<string, object>(),
                    System.Text.Json.JsonSerializer.Serialize(orderMessage));
                // // 短信通知
                _rabbitProducer.Publish(RabbitConstant.SMS_EXCHANGE, "",
                    new Dictionary<string, object>(),
                    System.Text.Json.JsonSerializer.Serialize(orderMessage));
                // // 通过死信队列来处理超时未处理的订单，把状态由0改成-1
                _rabbitProducer.Publish(RabbitConstant.DELAY_EXCHANGE, RabbitConstant.DELAY_ROUTING_KEY,
                    new Dictionary<string, object>()
                    {
                    {"x-delay", 1000*20 }
                    },
                    System.Text.Json.JsonSerializer.Serialize(orderMessage));
            }
            else
            {
                Console.WriteLine($"商品{seckillMessage.GoodsId}数据库库存为零，无法扣减库存");
            }
        }
    }
}

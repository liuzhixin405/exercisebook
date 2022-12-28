using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeMan.Seckill.Base.RabbitMq.Message;
using CodeMan.Seckill.Base.Redis;
using CodeMan.Seckill.Common.Utils;
using CodeMan.Seckill.Entities.Models;
using CodeMan.Seckill.HttpApi.Order.service;
using CodeMan.Seckill.Service.service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CodeMan.Seckill.HttpApi.Order.Controllers
{
    [Route("api/seckill")]
    [ApiController] 
    public class SeckillController : ControllerBase
    {
        private readonly ILogger<SeckillController> _logger;
        private readonly IOrderService _orderService;
        private readonly ISeckillGoodsService _seckillGoodsService;
        private readonly RedisHelper _redis;
        // 用于标识库存是否秒杀完毕，减少对缓存Redis的访问
        private static Dictionary<int, bool> _localOverMap = new Dictionary<int, bool>();

        public SeckillController(ILogger<SeckillController> logger, 
            IOrderService orderService, 
            ISeckillGoodsService seckillGoodsService, 
            RedisHelper redis)
        {
            _logger = logger;
            _orderService = orderService;
            _redis = redis;
            _seckillGoodsService = seckillGoodsService;
        }

        [HttpGet("/seckill")]
        public IActionResult Seckill(int userId, int goodsId)
        {
            if (_localOverMap.Count > 0)
            {
                var over = _localOverMap[goodsId];
                if (over)
                {
                    _logger.LogInformation($"商品:{goodsId}---商品已经秒杀完毕");
                    return StatusCode(ErrorCode.SECKILL_OVER, "商品已经秒杀完毕");
                }
            }

            var orderFlag = _redis.Get(RedisConstant.SECKILL_USER_GOODS + userId + "_" + goodsId);
            if (!orderFlag.IsNullOrEmpty)
            {
                _logger.LogInformation($"用户:{userId}---不能重复秒杀商品---商品:{goodsId}");
                return StatusCode(ErrorCode.REPEAT_SECKILL, "不能重复秒杀");
            }

            long stock = _redis.Decr(RedisConstant.SECKILL_GOODS_STOCK + goodsId);
            if (stock < 0)
            {
                Console.WriteLine("减库存商品已经秒杀完毕");
                _localOverMap[goodsId] = true;
                _redis.Incr(RedisConstant.SECKILL_GOODS_STOCK + goodsId);
                return StatusCode(ErrorCode.SECKILL_OVER, "商品已经秒杀完毕");
            }

            //入队
            SeckillMessage seckillMessage = new SeckillMessage();
            seckillMessage.UserId = userId;
            seckillMessage.GoodsId = goodsId;
            _seckillGoodsService.SendSeckillMessage(seckillMessage);
            return Ok("成功");
        }

        [HttpGet("test")]
        public IActionResult Test(string content)
        {
            //_redis.Set("hello", "CodeMan");
            _seckillGoodsService.SendMessage(content);
            //Console.WriteLine("111");
            return Ok("成功");
        }

        [HttpGet("reset")]
        public IActionResult Reset()
        {
            foreach (var seckillGoods in _seckillGoodsService.Reset())
            {
                _redis.Set(RedisConstant.SECKILL_GOODS_STOCK + seckillGoods.GoodsId, 10);
                _seckillGoodsService.ResetStock(seckillGoods.GoodsId);
                _localOverMap[seckillGoods.GoodsId] = false;
                //_localOverMap.Add(seckillGoods.GoodsId, false);
            }

            return Ok();
        }

    }
}

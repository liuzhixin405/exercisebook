using Cat.Seckill.Base.EFCore.Service;
using Cat.Seckill.Base.Redis;
using Cat.Seckill.Entities.BaseRepository;
using Cat.Seckill.Entities.Models;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Linq.Expressions;

namespace Cat.Seckill.HttpApi.Order.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SeckillController : ControllerBase
    {
       
        private readonly IUserService userService;
        private readonly ISeckillService seckillService;
        private readonly RedisHelper _redisHelper;
        private static Dictionary<int, bool> _localOverMap = new Dictionary<int, bool>();
        private readonly ILogger<SeckillController> logger;
        public SeckillController(IUserService userService, 
            ISeckillService seckillService, 
            ILogger<SeckillController> logger,
            RedisHelper redisHelper)
        {
           
            this.userService = userService;
            this.seckillService = seckillService;
            this.logger = logger;
            this._redisHelper= redisHelper;
        }

        [HttpPost]
        public async Task<bool> CreateSkillGoods(SeckillGoods seckillGoods)
        {
           return await seckillService.Create(seckillGoods);
        }

        [HttpPost]
        public async Task<IEnumerable<SeckillGoods>> GetSkillGoods()
        {
           var result =await seckillService.FindSeckillGoods();
            return result.ToList();
        }

        [HttpPost]
        public async Task<bool> CreateUser(string name,string pass,string email,string phone)
        {
           return await userService.CreateUser(name, pass, email, phone);
        }
        /// <summary>
        /// 秒杀商品要提前放到redis里面去
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="goodsId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Seckill(int userId,int goodsId)
        {
            if(_localOverMap.Count > 0)
            {
                var over = _localOverMap[goodsId];
                if (over)
                {
                    logger.LogInformation($"商品:{goodsId}---商品已经秒杀完毕");
                    return StatusCode(ErrorCode.SECKILL_OVER, "商品已经秒杀完毕");
                }
            }
            var orderFlag = _redisHelper.Get(RedisConstant.SECKILL_USER_GOODS + userId + "_" + goodsId);
            if (!orderFlag.IsNullOrEmpty)
            {
                logger.LogInformation($"用户:{userId}---不能重复秒杀商品---商品:{goodsId}");
                return StatusCode(ErrorCode.REPEAT_SECKILL, "不能重复秒杀");
            }
            long stock = _redisHelper.Decr(RedisConstant.SECKILL_GOODS_STOCK + goodsId);
            if (stock < 0)
            {
                Console.WriteLine("减库存商品已经秒杀完毕");
                _localOverMap[goodsId] = true;
                _redisHelper.Incr(RedisConstant.SECKILL_GOODS_STOCK + goodsId);
                return StatusCode(ErrorCode.SECKILL_OVER, "商品已经秒杀完毕");
            }
            await seckillService.Seckill(userId, goodsId);
            return Ok("success");
        }
    }
}

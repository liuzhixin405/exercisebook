using System.Collections.Generic;
using CodeMan.Seckill.Base.RabbitMq;
using CodeMan.Seckill.Base.RabbitMq.Message;
using CodeMan.Seckill.Entities.Models;
using CodeMan.Seckill.Service.Repository;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CodeMan.Seckill.HttpApi.Order.service
{
    public class SeckillGoodsService : ISeckillGoodsService
    {
        private ILogger<SeckillGoodsService> _logger;
        private readonly IRepositoryWrapper _repositoryWrapper;
        private readonly IRabbitProducer _rabbitProducer;

        public SeckillGoodsService(ILogger<SeckillGoodsService> logger, IRepositoryWrapper repositoryWrapper, IRabbitProducer rabbitProducer)
        {
            _logger = logger;
            _repositoryWrapper = repositoryWrapper;
            _rabbitProducer = rabbitProducer;
        }


        public IEnumerable<SeckillGoods> Reset()
        {
            return _repositoryWrapper.SeckillGoods.GetAllSeckillGoods();
        }

        public void SendSeckillMessage(SeckillMessage seckillMessage)
        {
            var body = JsonConvert.SerializeObject(seckillMessage);
            _logger.LogInformation($"send message:{body}");
            _rabbitProducer.Publish(RabbitConstant.SECKILL_EXCHANGE, "", 
                new Dictionary<string, object>(), body);
        }

        public void SendMessage(string content)
        {
            _rabbitProducer.Publish(RabbitConstant.TEST_EXCHANGE, "" , 
                new Dictionary<string, object>(), content);
        }

        public void ResetStock(int goodsId)
        {
            SeckillGoods seckillGoods = _repositoryWrapper.SeckillGoods.GetSeckillGoodsByGoodsId(goodsId);
            seckillGoods.StockCount = 10;
            _repositoryWrapper.SeckillGoods.Update(seckillGoods);
            _repositoryWrapper.Save();
        }
    }
}
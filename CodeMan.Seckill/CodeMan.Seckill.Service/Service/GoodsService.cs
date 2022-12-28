using CodeMan.Seckill.Entities.Models;
using CodeMan.Seckill.Service.Repository;

namespace CodeMan.Seckill.Service.service
{
    public class GoodsService : IGoodsService
    {

        private readonly IRepositoryWrapper _repositoryWrapper;

        public GoodsService(IRepositoryWrapper repositoryWrapper)
        {
            _repositoryWrapper = repositoryWrapper;
        }

        public bool ReduceStock(int goodsId)
        {
            SeckillGoods seckillGoods = _repositoryWrapper.SeckillGoods.GetSeckillGoodsByGoodsId(goodsId);
            seckillGoods.StockCount--;
            if (!(seckillGoods.StockCount < 0))
            {
                _repositoryWrapper.SeckillGoods.Update(seckillGoods);
                //_repositoryWrapper.Save();
                return true;
            }
            return false;
        }
    }
}
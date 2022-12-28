using System.Collections.Generic;
using CodeMan.Seckill.Entities.Models;

namespace CodeMan.Seckill.Service.Repository
{

    public interface ISeckillGoodsRepository : IRepositoryBase<SeckillGoods>
    {
        SeckillGoods GetSeckillGoodsByGoodsId(int goodsId);

        public IEnumerable<SeckillGoods> GetAllSeckillGoods();

    }
}
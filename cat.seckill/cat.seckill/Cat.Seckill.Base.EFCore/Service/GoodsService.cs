using Cat.Seckill.Entities.BaseRepository;
using Cat.Seckill.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cat.Seckill.Base.EFCore.Service
{
    public class GoodsService : IGoodsService
    {
        private readonly IRepository<SeckillGoods> repository;
        private readonly IRepository<Goods> goodsRepository;
        public GoodsService(IRepository<SeckillGoods> repository, IRepository<Goods> goodsRepository)
        {
            this.repository =repository;
            this.goodsRepository=goodsRepository;
        }

        public async Task<Goods> FindById(int id)
        {
            var res = await goodsRepository.GetById(id);
            return res;
        }

        public async Task<bool> ReduceStock(int id)
        {
            SeckillGoods seckillGoods =await repository.GetById(id);
            seckillGoods.StockCount--;
            if(!(seckillGoods.StockCount < 0))
            {
               await repository.Update(seckillGoods);
                return true;
            }
            return false;
        }
    }
}

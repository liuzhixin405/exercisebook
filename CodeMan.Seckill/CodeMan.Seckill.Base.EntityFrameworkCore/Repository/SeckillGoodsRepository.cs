using System.Collections.Generic;
using System.Linq;
using CodeMan.Seckill.Entities.Models;
using CodeMan.Seckill.Service.Repository;
using Microsoft.EntityFrameworkCore;

namespace CodeMan.Seckill.Base.EntityFrameworkCore.Repository
{
    public class SeckillGoodsRepository : RepositoryBase<SeckillGoods>, ISeckillGoodsRepository
    {
        public SeckillGoodsRepository(SeckillDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public void Update()
        {
            Update();
        }

        public SeckillGoods GetSeckillGoodsByGoodsId(int goodsId)
        {
            SeckillGoods seckillGoods = FindByCondition(seckillGoods => seckillGoods.GoodsId == goodsId)
                .SingleOrDefault();
            SeckillDbContext.Entry(seckillGoods).State = EntityState.Detached;
            return seckillGoods;
        }

        public IEnumerable<SeckillGoods> GetAllSeckillGoods()
        {
            return FindAll()
                .OrderBy(seckillGoods => seckillGoods.SeckillGoodsId)
                .ToList();
        }
    }
}
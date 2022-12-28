using System.Linq;
using CodeMan.Seckill.Entities.Models;
using CodeMan.Seckill.Service.Repository;

namespace CodeMan.Seckill.Base.EntityFrameworkCore.Repository
{
    public class GoodsRepository : RepositoryBase<Goods>, IGoodsRepository
    {

        public GoodsRepository(SeckillDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public Goods GetGoodsById(int goodsId)
        {
            return FindByCondition(goods => goods.GoodsId == goodsId)
                .FirstOrDefault();
        }
    }
}
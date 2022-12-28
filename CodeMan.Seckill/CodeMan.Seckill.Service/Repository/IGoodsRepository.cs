using CodeMan.Seckill.Entities.Models;

namespace CodeMan.Seckill.Service.Repository
{
    public interface IGoodsRepository : IRepositoryBase<Goods>
    {
        Goods GetGoodsById(int goodsId);
    }
}
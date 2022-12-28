using CodeMan.Seckill.Service.Repository;

namespace CodeMan.Seckill.Base.EntityFrameworkCore.Repository
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly SeckillDbContext _seckillDbContext;
        private IGoodsRepository _goods;
        private ISeckillGoodsRepository _seckillGoods;
        private IOrderInfoRepository _orderInfo;
        private IUserRepository _user;

        public IGoodsRepository Goods { get{ return _goods ??= new GoodsRepository(_seckillDbContext); } }
        public ISeckillGoodsRepository SeckillGoods { get { return _seckillGoods ??= new SeckillGoodsRepository(_seckillDbContext); } }
        public IOrderInfoRepository OrderInfo { get { return _orderInfo ??= new OrderRepository(_seckillDbContext); } }
        public IUserRepository User { get { return _user ??= new UserRepository(_seckillDbContext); } }

        public RepositoryWrapper(SeckillDbContext seckillDbContext)
        {
            _seckillDbContext = seckillDbContext;
        }

        public void Save()
        {
            _seckillDbContext.SaveChanges();
        }
    }
}
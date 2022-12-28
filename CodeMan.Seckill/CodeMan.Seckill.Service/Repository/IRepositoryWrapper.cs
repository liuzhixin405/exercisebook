namespace CodeMan.Seckill.Service.Repository
{
    public interface IRepositoryWrapper
    {
        IGoodsRepository Goods { get; }

        ISeckillGoodsRepository SeckillGoods { get; }

        IOrderInfoRepository OrderInfo { get; }

        IUserRepository User { get; }

        void Save();
    }
}
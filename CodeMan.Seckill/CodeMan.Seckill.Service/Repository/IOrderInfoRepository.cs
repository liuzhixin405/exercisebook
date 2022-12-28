using CodeMan.Seckill.Entities.Models;

namespace CodeMan.Seckill.Service.Repository
{
    public interface IOrderInfoRepository : IRepositoryBase<OrderInfo>
    {
        public void CreateOrder(OrderInfo orderInfo);
    }
}
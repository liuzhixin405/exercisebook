using CodeMan.Seckill.Entities.Models;
using CodeMan.Seckill.Service.Repository;

namespace CodeMan.Seckill.Base.EntityFrameworkCore.Repository
{
    public class OrderRepository : RepositoryBase<OrderInfo>, IOrderInfoRepository
    {
        public OrderRepository(SeckillDbContext repositoryContext) : base(repositoryContext)
        {
        }

        public void CreateOrder(OrderInfo orderInfo)
        {
            Create(orderInfo);
        }
    }
}
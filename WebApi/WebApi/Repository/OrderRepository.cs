using WebApi.Data;
using WebApi.Models;

namespace WebApi.Repository
{
    public class OrderRepository : RepositoryBase<Order,int>
    {
        public OrderRepository(OrderDbContext orderDbContext):base(orderDbContext)
        {

        }
        //自己的逻辑
    }
}

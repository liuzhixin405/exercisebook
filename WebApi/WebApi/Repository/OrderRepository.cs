using WebApi.Data;
using WebApi.Models;

namespace WebApi.Repository
{
    public class OrderRepository : RepositoryBase<Order>,IRepository<Order>
    {
        public OrderRepository(OrderDbContext orderDbContext):base(orderDbContext)
        {

        }
    }
}

using WebApi.Data;
using WebApi.Models;

namespace WebApi.Repository
{
    public class OrderRepository : RepositoryBase<Order,int>
    {
        public OrderRepository(DbFactory factory):base(factory)
        {

        }
        //自己的逻辑
    }
}

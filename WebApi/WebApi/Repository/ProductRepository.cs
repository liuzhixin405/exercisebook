using WebApi.Data;
using WebApi.Models;

namespace WebApi.Repository
{
    public class ProductRepository : RepositoryBase<Product, int>
    {
        public ProductRepository(DbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}

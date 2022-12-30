using eapi.Data;
using eapi.Models;

namespace eapi.Repositories
{
    public class ProductRepository : ReposioryBase<Product>
    {
        public ProductRepository(ProductDbContext dbContext) : base(dbContext)
        {
        }
    }
}

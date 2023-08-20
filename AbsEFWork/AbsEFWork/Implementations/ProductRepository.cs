using BaseEntityFramework.Implementations.Entitys;
using IServiceEF.DefaultImplement;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace BaseEntityFramework.Implementations
{
    public class ProductRepository : DefaultEfCoreRepository<ProductDbContext,Product>
    {
       
        public ProductRepository(ProductDbContext context) : base(context)
        {
        }
    }
}

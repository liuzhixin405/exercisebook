using BaseEntityFramework.Implementations.Entitys;
using Microsoft.EntityFrameworkCore;

namespace BaseEntityFramework.Implementations
{
    public class ProductDbContext:DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> dbContextOptions):base(dbContextOptions)
        {

        } 
        public DbSet<Product> Products { get; set; }
    }
}

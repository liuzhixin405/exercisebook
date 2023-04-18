using Microsoft.EntityFrameworkCore;

namespace webapi
{
    public class ProductDbContext:DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> contextOptions):base(contextOptions)
        {
                
        }

        public DbSet<Product> Products { get; set; }
    }
}

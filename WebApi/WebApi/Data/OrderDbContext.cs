using Microsoft.EntityFrameworkCore;
using WebApi.Models;

namespace WebApi.Data
{
    public class OrderDbContext:DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> contextOptions):base(contextOptions)
        {

        }

        public DbSet<Order> Orders =>Set<Order>();
      
    }
}

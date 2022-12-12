using BlazorApp.Shared;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Data
{
    public class CustomerDbContext:DbContext
    {
        public CustomerDbContext(DbContextOptions<CustomerDbContext> options):base(options) { }
        
        public DbSet<Customer> Customer=>Set<Customer>();
        
    }
}

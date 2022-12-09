using Microsoft.EntityFrameworkCore;
using RazorDemo.Models;
namespace RazorDemo.Data
{
    public class CustomerDbContext : DbContext
    {   
        public CustomerDbContext(DbContextOptions<CustomerDbContext> options):base(options)
        {
            
        }

        public DbSet<Customer> Customer=>Set<Customer>();
    }
}
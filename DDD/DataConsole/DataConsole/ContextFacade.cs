using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace DataConsole
{
    public class ContextFacade : DbContext
    {
        public ContextFacade(DbContextOptions<ContextFacade> contextOptions) : base(contextOptions)
        {
            Order = base.Set<Order>();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>();
            modelBuilder.Entity<Department>();
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<Order> Order { get; private set; }
    }

    public class Customer
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Department
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int ParentId { get; set; }
    }

    public class Order
    {
        [Key]
        public int Id { get; set; }
        public int CustomerId { get; set; }
    }
}

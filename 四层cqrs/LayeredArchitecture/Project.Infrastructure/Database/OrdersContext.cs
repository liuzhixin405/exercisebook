using Microsoft.EntityFrameworkCore;
using Project.Domain.Customers;
using Project.Domain.Payments;
using Project.Domain.Products;
using Project.Infrastructure.Processing.InternalCommands;
using Project.Infrastructure.Processing.Outbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.Database
{
    public class OrdersContext:DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<OutboxMessage> OutboxMessages { get; set; }
        public DbSet<InternalCommand> InternalCommands { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public OrdersContext(DbContextOptions options):base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrdersContext).Assembly);
        }

    }
}

using Microsoft.EntityFrameworkCore;
using spot.Domain.Accounts.Entities;
using spot.Domain.Orders.Entities;
using spot.Domain.Products.Entities;
using spot.Domain.Trades.Entities;
using System.Threading.Tasks;

namespace spot.Infrastructure.Persistence.Contexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Trade> Trades { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Product entity
            modelBuilder.Entity<Product>()
                .ToTable("products")
                .HasKey(p => p.Id);

            // Configure Account entity
            modelBuilder.Entity<Account>()
                .ToTable("accounts")
                .HasKey(a => a.Id);

            // Configure Order entity
            modelBuilder.Entity<Order>()
                .ToTable("orders")
                .HasKey(o => o.Id);

            // Configure Trade entity
            modelBuilder.Entity<Trade>()
                .ToTable("trades")
                .HasKey(t => t.Id);

            base.OnModelCreating(modelBuilder);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }
    }
}
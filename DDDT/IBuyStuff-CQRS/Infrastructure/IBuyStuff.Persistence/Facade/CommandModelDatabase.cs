using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBuyStuff.Domain.Customers;
using IBuyStuff.Domain.Misc;
using IBuyStuff.Domain.Orders;
using IBuyStuff.Domain.Products;
using IBuyStuff.Domain.Shared;
using IBuyStuff.Persistence.Mappings;
using IBuyStuff.Persistence.Utils;
using Microsoft.EntityFrameworkCore;

namespace IBuyStuff.Persistence.Facade
{
    public class CommandModelDatabase : DbContext
    {      

        public const string DEFAULT_SCHEMA = "ordering";
        public CommandModelDatabase(DbContextOptions<CommandModelDatabase> options): base(options) // specify here conn-string entry if using SQL Server
        {
            Products = base.Set<Product>();
            Customers = base.Set<Customer>();
            Orders = base.Set<Order>();
           
            FidelityCards = base.Set<FidelityCard>();
            Subscribers = base.Set<Subscriber>();

            //new SampleAppInitializer().Seed(this);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            EnsureDatabase(modelBuilder);           
        }

        public DbSet<Order> Orders { get; private set; }

        public DbSet<Customer> Customers { get; private set; }

      

        public DbSet<Product> Products { get; private set; }

        public DbSet<FidelityCard> FidelityCards { get; private set; }

        public DbSet<Subscriber> Subscribers { get; private set; }

        #region Database modeling
        public static void EnsureDatabase(ModelBuilder modelBuilder)
        {
            
            modelBuilder.Owned<Money>();
            modelBuilder.Owned<Address>();
            modelBuilder.Owned<CreditCard>();
           
            
            //modelBuilder.ApplyConfiguration(new ExpiryDateMap());
            modelBuilder.ApplyConfiguration(new FidelityCardMap());
            modelBuilder.ApplyConfiguration(new OrderMap());
            //modelBuilder.ApplyConfiguration(new CurrencyMap());
            modelBuilder.ApplyConfiguration(new OrderItemMap());
            modelBuilder.ApplyConfiguration(new CustomerMap());
            modelBuilder.ApplyConfiguration(new ProductMap());
        }
        #endregion
    }
}

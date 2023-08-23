using IBuyStuff.Domain.Orders;
using IBuyStuff.Domain.Products;
using IBuyStuff.QueryModel.Persistence.Mappings;
using Microsoft.EntityFrameworkCore;

namespace IBuyStuff.QueryModel.Persistence
{
    public class QueryModelDatabase : DbContext, IQueryModelDatabase
    {
        public QueryModelDatabase(DbContextOptions<QueryModelDatabase> options):base(options)
        {
            _products = base.Set<Product>();
            _orders = base.Set<Order>();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Owned<Money>();
            //modelBuilder.Owned<Address>();
            //modelBuilder.Owned<CreditCard>();

            modelBuilder.ApplyConfiguration(new ExpiryDateMap());
            //modelBuilder.ApplyConfiguration(new FidelityCardMap());
            modelBuilder.ApplyConfiguration(new OrderMap());
            modelBuilder.ApplyConfiguration(new CurrencyMap());
            //modelBuilder.ApplyConfiguration(new OrderItemMap());
            //modelBuilder.ApplyConfiguration(new CustomerMap());
          

        }
        private readonly DbSet<Order> _orders = null;
        private readonly DbSet<Product> _products = null;

        public IQueryable<Order> Orders => this._orders.Include("Items").Include("Items.Product");
        public IQueryable<Product> Products => this._products;
    }
}
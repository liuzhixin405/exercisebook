using IBuyStuff.Domain.Customers;
using IBuyStuff.Domain.Orders;
using IBuyStuff.Domain.Products;
using IBuyStuff.Domain.Shared;
using IBuyStuff.Persistence.Mappings;
using Microsoft.EntityFrameworkCore;

namespace IBuyStuff.QueryModel.Persistence
{
    public class QueryModelDatabase : DbContext, IQueryModelDatabase
    {
        public QueryModelDatabase(DbContextOptions<QueryModelDatabase> options) : base(options)
        {
            _products = base.Set<Product>();
            _orders = base.Set<Order>();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Owned<Money>(); //值对象
            modelBuilder.Owned<Address>();//值对象
            modelBuilder.Owned<CreditCard>();//值对象


            //modelBuilder.ApplyConfiguration(new ExpiryDateMap());
            modelBuilder.ApplyConfiguration(new FidelityCardMap());
            modelBuilder.ApplyConfiguration(new OrderMap());
            //modelBuilder.ApplyConfiguration(new CurrencyMap());
            modelBuilder.ApplyConfiguration(new OrderItemMap());
            modelBuilder.ApplyConfiguration(new CustomerMap());
            modelBuilder.ApplyConfiguration(new ProductMap());

        }
        private readonly DbSet<Order> _orders = null;
        private readonly DbSet<Product> _products = null;

        public IQueryable<Order> Orders=>this._orders.Include(o => o.Items).ThenInclude(t => t.Product).AsNoTracking();   
        public IQueryable<Product> Products => this._products.AsNoTracking();
        public IQueryable<Order> OrderIncludeNot => this._orders.AsNoTracking();
    }
}
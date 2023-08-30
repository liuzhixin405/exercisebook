using IBuyStuff.Domain.Orders;
using IBuyStuff.Domain.Products;
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
        }
        private readonly DbSet<Order> _orders = null;
        private readonly DbSet<Product> _products = null;

        public IQueryable<Order> Orders => this._orders.Include("Items").Include("Items.Product");
        public IQueryable<Product> Products => this._products;
    }
}
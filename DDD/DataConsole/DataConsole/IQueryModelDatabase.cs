using DataConsole.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DataConsole
{
    public interface IQueryModelDatabase
    {
        IQueryable<Order> Orders { get; }
        IQueryable<Product> Products { get; }
    }

    public class QueryModelDatabase: DbContext,IQueryModelDatabase
    {
        private readonly DbSet<Order> _orders=null;
        private readonly DbSet<Product> _products = null;
        public QueryModelDatabase(DbContextOptions<QueryModelDatabase> databaseOptions):base(databaseOptions)
        {
            _orders = base.Set<Order>();
            _products = base.Set<Product>();
        }
        public IQueryable<Order> Orders => this._orders;

        public IQueryable<Product> Products => this._products;
    }
}

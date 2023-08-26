using IBuyStuff.Domain.Orders;
using IBuyStuff.Domain.Products;

namespace IBuyStuff.QueryModel
{
    public interface IQueryModelDatabase
    {   
        IQueryable<Order> Orders { get; }
        IQueryable<Product> Products { get; }
    }
}
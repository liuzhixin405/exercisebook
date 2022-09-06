using System.Collections.ObjectModel;

namespace CqrsWithEs.Domain.Product
{
    public interface IProductRepository
    {
        void Add(Product product);
        Task<Product> WithCode(string code);
        Task<ReadOnlyCollection<Product>> All();
    }
}

using CqrsWithEs.Domain.Product;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace CqrsWithEs.DataAccess
{
    public class InMemoryProductsRepository: IProductRepository
    {
        private readonly IDictionary<string, Product> products = new ConcurrentDictionary<string, Product>();
        public void Add(Product product)
        {
            products.Add(product.Code, product);
        }

        public Task<Product> WithCode(string code)
        {
            return Task.FromResult(products[code]);
        }

        public Task<ReadOnlyCollection<Product>> All()
        {
            var allProducts = products.Values.ToList().AsReadOnly();
            return Task.FromResult(allProducts);
        }
    }
}

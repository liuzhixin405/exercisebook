using spot.Application.Interfaces.Products;
using spot.Application.Interfaces.Repositories;
using spot.Domain.Products.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spot.Infrastructure.Persistence.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        public Task<IReadOnlyList<Product>> GetAllProducts()
        {
            return _productRepository.GetAllAsync();
        }

        public Task<Product> GetProductByIdQuery(string id)
        {
            return _productRepository.GetByIdAsync(id);
        }
    }
}

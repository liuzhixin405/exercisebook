using WebApi.Data;
using WebApi.Models;
using WebApi.Repository;

namespace WebApi.Service
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product,int> _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ProductService(IRepository<Product,int> productRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Create(string sku, int count)
        {
           await _productRepository.Add(new Product { Sku = sku,Count= count, Price=10});
            await _unitOfWork.CommitAsync();
        }
    }
}

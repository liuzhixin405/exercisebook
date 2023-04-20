using chatgptwriteproject.Context;
using chatgptwriteproject.Models;
using chatgptwriteproject.Repositories;

namespace chatgptwriteproject.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _repository;
      
        public ProductService(IRepository<Product> repository)
        {
            _repository = repository;
        }
        public async Task Add(Product product)
        {
            _repository.Add(product);
            await _repository.UnitOfWork.SaveChangeAsync();
        }

        public async Task Delete(Product entity)
        {
            _repository.Delete(entity);
            await _repository.UnitOfWork.SaveChangeAsync();
        }

        public Task<IEnumerable<Product>> GetAll()
        {
            return _repository.GetAll();
        }

        public ValueTask<Product> GetById(int id)
        {
            return _repository.GetById(id);
        }

        public async Task Update(Product entity)
        {
            _repository.Update(entity);
            await _repository.UnitOfWork.SaveChangeAsync();
        }
    }
}

using chatgptwriteproject.Models;

namespace chatgptwriteproject.Services
{
    public interface IProductService
    {
        Task Add(Product product);
        Task<IEnumerable<Product>> GetAll();
        ValueTask<Product> GetById(int id);
        Task Update(Product entity);
        Task Delete(Product entity);
    }
}

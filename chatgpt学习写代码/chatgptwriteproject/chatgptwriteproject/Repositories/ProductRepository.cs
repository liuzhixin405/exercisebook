using chatgptwriteproject.Context;
using chatgptwriteproject.Models;
using Microsoft.EntityFrameworkCore;

namespace chatgptwriteproject.Repositories
{
    public class ProductRepository : IRepository<Product>
    {
        private readonly ApplicationDbContext _dbContext;

        public IUnitOfWork UnitOfWork => _dbContext;

        public ProductRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            return await _dbContext.Products.ToListAsync();
        }

        public ValueTask<Product> GetById(int id)
        {
            return _dbContext.Products.FindAsync(id);
        }

        public void Add(Product product)
        {
            _dbContext.Products.Add(product);
           
        }

        public void Update(Product product)
        {
            _dbContext.Entry(product).State = EntityState.Modified;
            _dbContext.Products.Update(product);
          
        }

        public void Delete(Product product)
        {
            _dbContext.Products.Remove(product);
        }
    }

}

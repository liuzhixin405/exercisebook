using Microsoft.EntityFrameworkCore;
using spot.Application.Interfaces.Repositories;
using spot.Domain.Products.Entities;
using spot.Infrastructure.Persistence.Contexts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace spot.Infrastructure.Persistence.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly DbSet<Product> _products;

        public ProductRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _products = dbContext.Set<Product>();
        }

        public async Task<IReadOnlyList<Product>> GetAllActiveProductsAsync()
        {
            return await _products.ToListAsync();
        }
    }
}
using chatgptwriteproject.BaseRepository;
using chatgptwriteproject.Context;
using chatgptwriteproject.DbFactories;
using chatgptwriteproject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace chatgptwriteproject.Repositories
{
    public class ProductRepository : RepositoryBase<ApplicationDbContext,Product>, IProductRepository
    {
        private readonly ApplicationDbContext _productDbContext;

        public ProductRepository(DbFactory<ApplicationDbContext> dbfactory) : base(dbfactory)
        {
            _productDbContext = dbfactory.Context;
        }

        public async ValueTask<Product> GetById(int id)
        {
            var result =await _productDbContext.Products.Where(x => x.Id == id).FirstOrDefaultAsync();
            return result;
        }

        public override IUnitOfWork GetUnitOfWork()
        {
            return _productDbContext;
        }
    }

}

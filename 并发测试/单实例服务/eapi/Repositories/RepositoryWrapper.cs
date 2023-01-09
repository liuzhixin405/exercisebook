using eapi.Data;
using eapi.interfaces.Models;

namespace eapi.Repositories
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly ProductDbContext _;
        private readonly ILogger<RepositoryWrapper> _logger;
        public RepositoryWrapper(ProductDbContext context, ILogger<RepositoryWrapper> logger)
        {
            _ = context;
            _logger = logger;
        }
        //public ProductDbContext context => _;
        public async Task Trans(Func<Task> func)
        {
            var trans =await _.Database.BeginTransactionAsync();
            try
            {
               await func();
               await  trans.CommitAsync();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                _logger.LogError(ex.Message);
            }
            finally
            {
                trans.Dispose();
            }
        }
        public IRepository<Order> OrderRepository => new OrderRepository(_);
        public IRepository<Product> ProductRepository => new ProductRepository(_);
    }
}

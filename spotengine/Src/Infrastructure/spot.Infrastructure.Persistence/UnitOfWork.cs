using spot.Application.Interfaces;
using spot.Infrastructure.Persistence.Contexts;
using System.Threading;
using System.Threading.Tasks;

namespace spot.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
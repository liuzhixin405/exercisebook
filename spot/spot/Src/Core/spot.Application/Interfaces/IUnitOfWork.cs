using System.Threading;
using System.Threading.Tasks;

namespace spot.Application.Interfaces
{
    public interface IUnitOfWork
    {
        Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}

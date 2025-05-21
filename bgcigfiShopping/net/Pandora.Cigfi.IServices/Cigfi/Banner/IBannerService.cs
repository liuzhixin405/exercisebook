using Pandora.Cigfi.Models.Cigfi;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pandora.Cigfi.IServices.Cigfi
{
    public interface IBannerService
    {
        Task<IEnumerable<Banner>> GetAllAsync();
        Task<Banner> GetByIdAsync(long id);
        Task AddAsync(Banner banner);
        Task UpdateAsync(Banner banner);
        Task DeleteAsync(IEnumerable<long> ids);
    }
}
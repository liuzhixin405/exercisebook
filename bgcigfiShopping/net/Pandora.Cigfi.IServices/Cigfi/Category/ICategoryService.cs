using Pandora.Cigfi.Models.Cigfi;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pandora.Cigfi.IServices.Cigfi
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryViewModel>> GetAllAsync();
        Task<CategoryViewModel> GetByIdAsync(long id);
        Task<bool> AddAsync(Category category);
        Task<bool> UpdateAsync(Category category);
        Task<bool> DeleteAsync(IEnumerable<long> ids);
    }
}

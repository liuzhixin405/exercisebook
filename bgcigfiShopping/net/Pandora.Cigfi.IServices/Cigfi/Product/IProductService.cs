using Pandora.Cigfi.Models.Cigfi.Product;
using Pandora.Cigfi.Models.ResponseMsg;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pandora.Cigfi.IServices.Cigfi
{
    public interface IProductService
    {
        Task<PagedResult<ProductViewModel>> GetPageListAsync(Hashtable queryHt, int page = 1, int limit = 20);
        Task<int> CountAsync(Hashtable queryHt);
        Task<ProductViewModel> GetByIdAsync(int id);
        Task<bool> AddAsync(Product model);
        Task<bool> UpdateAsync(Product model);
        Task<bool> DeleteAsync(List<int> ids);
    }
}

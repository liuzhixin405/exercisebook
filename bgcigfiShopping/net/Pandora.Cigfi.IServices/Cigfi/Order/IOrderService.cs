using Pandora.Cigfi.Models.Cigfi.Order;
using Pandora.Cigfi.Models.ResponseMsg;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pandora.Cigfi.IServices.Cigfi
{
    public interface IOrderService
    {
        Task<PagedResult<OrderViewModel>> GetPageListAsync(Hashtable queryHt, int page = 1, int limit = 20);
        Task<int> CountAsync(Hashtable queryHt);
        Task<OrderViewModel> GetByIdAsync(int id);
        Task<bool> UpdateAsync(OrderViewModel model);
        Task<bool> DeleteAsync(List<int> ids);
    }
}

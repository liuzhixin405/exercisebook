using Pandora.Cigfi.Models.Cigfi.Invitation;
using Pandora.Cigfi.Models.Cigfi.Rebate;
using Pandora.Cigfi.Models.ResponseMsg;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandora.Cigfi.IServices.Cigfi
{
    public interface IRebateService
    {
        Task<PagedResult<RebateViewModel>> GetPageListAsync(Hashtable queryHt, int page = 1, int limit = 20);
        Task<int> CountAsync(Hashtable queryHt);
        Task<RebateViewModel> GetByIdAsync(int id);
        Task<bool> AddAsync(Rebate model);
        Task<bool> UpdateAsync(Rebate model);
        Task<bool> DeleteAsync(List<int> ids);
    }
}

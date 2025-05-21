
using Pandora.Cigfi.Models.Cigfi.Invitation;
using Pandora.Cigfi.Models.ResponseMsg;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandora.Cigfi.IServices.Cigfi
{
    public  interface IInvitationService
    {
        Task<PagedResult<CigfiMemberViewModel>> GetPageListAsync(Hashtable queryHt, int page = 1, int limit = 20);
        Task<int> CountAsync(Hashtable queryHt);
        Task<CigfiMemberViewModel> GetByIdAsync(long id);
        Task<bool> AddAsync(CigfiMember model);
        Task<bool> UpdateAsync(CigfiMember model);
        Task<bool> DeleteAsync(List<long> ids);
    }
}

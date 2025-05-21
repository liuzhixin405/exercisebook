using Pandora.Cigfi.Common.Requests;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pandora.Cigfi.IServices
{
    public interface IBaseRequestService<T>:IBaseService_NoPaging<T>
    {

        Task<T> FindEntity(QitemCollection q);
        Task<int> CountAsync(AdminCPRequest fxhRequest);
        Task<IEnumerable<T>> GetPageListAsync(AdminCPRequest fxhRequest);
    }
}

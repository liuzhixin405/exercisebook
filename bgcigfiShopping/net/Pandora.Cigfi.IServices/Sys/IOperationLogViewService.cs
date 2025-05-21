using Pandora.Cigfi.Models.Sys;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pandora.Cigfi.IServices.Sys
{
    public interface IOperationLogViewService : IBaseService<Sys_ReviewLogViewModel>
    {
        /// <summary>
        /// 根据Id取详情
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        Task<Sys_ReviewLogViewModel> GetModelById(int Id);
    }
}

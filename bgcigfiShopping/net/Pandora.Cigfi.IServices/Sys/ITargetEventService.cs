
using Dapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FXH.Common.DapperService;
using Pandora.Cigfi.IServices; 
using Pandora.Cigfi.Models.Sys;

namespace Pandora.Cigfi.Services.Sys
{
    public interface ITargetEventService : IBaseService<Sys_TargetEventModel>
    {
        /// <summary>
        /// 是否存在相同eventKey值的记录
        /// </summary>
        /// <param name="eventKey"></param>
        /// <returns></returns>
        Task<bool> ExistsAsync(string eventKey);
    }
}

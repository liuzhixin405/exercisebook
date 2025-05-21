
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
    public interface IAdminLogService : IBaseService<Sys_AdminLogModel>
    {
        /// <summary>
        /// 登录失败的次数
        /// </summary>
        /// <param name="ip">ip地址</param>
        /// <param name="beginLoginTime">开始计算的登录时间</param>
        /// <returns></returns>
        Task<int> CountFailLoginTimeAsync(  string ip,DateTime beginLoginTime);
    }
}

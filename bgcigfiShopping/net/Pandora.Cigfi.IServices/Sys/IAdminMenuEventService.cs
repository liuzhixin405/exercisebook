
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
    public interface IAdminMenuEventService : IBaseService<Sys_AdminMenuEventModel>
    {
        /// <summary>
        /// 读取菜单事件列表 Dictionary<菜单ID, List<Sys_AdminMenuEventModel>>
        /// </summary>
        /// <returns></returns>
        Task<Dictionary<int, List<Sys_AdminMenuEventModel>> > GetMenuEventListAsync();

        /// <summary>
        /// 读取菜单事件列表 Dictionary<主键id, List<Sys_AdminMenuEventModel>>
        /// </summary>
        /// <returns></returns>
        Task<Dictionary<int,  Sys_AdminMenuEventModel> > GetIdMenuEventDcAsync();

        /// <summary>
        /// 读取菜单事件列表
        /// </summary>
        /// <param name="menuId">菜单id</param>
        /// <returns></returns>
        Task<List<Sys_AdminMenuEventModel>> GetMenuEventListAsync(int menuId);
    }
}

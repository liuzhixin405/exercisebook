
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
    public interface IAdminMenuService : IBaseService<Sys_AdminMenuModel>
    {
        /// <summary>
        /// 是否存在相同menuKey值的记录
        /// </summary>
        /// <param name="menuKey"></param>
        /// <param name="menuId">不等于此menuId</param>
        /// <returns></returns>
        Task<bool> ExistsAsync(string menuKey,int menuId=0);

        /// <summary>
        /// 新增菜单
        /// </summary>
        /// <param name="model"></param>
        /// <param name="menuEventList"></param>
        /// <returns></returns>
        Task<bool> InsertMenuAsync(Sys_AdminMenuModel model, List<Sys_AdminMenuEventModel> menuEventList);

        /// <summary>
        /// 更新菜单
        /// </summary>
        /// <param name="model"></param>
        /// <param name="menuEventList"></param>
        /// <returns></returns>
        Task<bool> UpdateMenuAsync(Sys_AdminMenuModel model, List<Sys_AdminMenuEventModel> menuEventList);


        /// <summary>
        /// 获取菜单树
        /// </summary>
        /// <param name="pid">上级ID，0为全部</param>
        /// <param name="maxLevel">最大级别0为1级类别；-1为所有下级</param>
        /// <param name="isIndentation">是否缩进</param>
        /// <param name="showhide">是否显示隐藏菜单</param>
        /// <returns></returns>
        Task<List<Sys_AdminMenuModel>> GetListTreeAsync(int pid, int maxLevel, bool isIndentation, bool showhide = true);

        /// <summary>
        /// 读取网站左侧导航栏菜单
        /// </summary>
        /// <returns></returns>
        Task<List<Sys_AdminMenuViewModel>> GetLeftMenuAsync();

        
    }
}

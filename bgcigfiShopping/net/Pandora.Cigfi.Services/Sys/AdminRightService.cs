using Pandora.Cigfi.IServices.Sys;
using Pandora.Cigfi.Models.Sys;
using FXH.Common.Logger;
using FXH.Web.Extensions.Http; 
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using Pandora.Cigfi.IServices;
using Pandora.Cigfi.Services.Sys;

namespace Pandora.Cigfi.Services 
{
    /// <summary>
    /// 管理员权限实现类
    /// </summary>
    public class AdminRightService: IAdminRightService
    {
        private IAdminService _adminService;
        private IAdminRolesService _adminRolesService; 
        public AdminRightService(IAdminService adminService, IAdminRolesService adminRolesService)
        {
            _adminService = adminService;
            _adminRolesService = adminRolesService;
        }

        #region 检查管理员有没有管理权限
        /// <summary>
        /// 检查管理员有没有管理权限
        /// </summary>
        /// <param name="eventKey">动作，如add、edit</param>
        /// <param name="menuKey">菜单名称，如article、product</param>
        /// <returns></returns>
        public async Task<bool> CheckAdminPower(string eventKey, string menuKey)
        {
            try
            {

                if (!await _adminService.IsLogin())
                {
                    return false;
                }

                Sys_AdminModel adminModel = await _adminService.GetAdminInfoAsync(AdminHelper.CurrentUserName);

                Sys_AdminRolesModel adminRoleModel = await _adminRolesService.GetByIdAsync<int>(adminModel.RoleId);
                //判断权限
           
                if (adminRoleModel.IsSuperAdmin != 1)
                {
                    //获取所有的菜单权限
                    if (string.IsNullOrEmpty(adminRoleModel.Powers))
                        return false;
                    IList<Sys_AdminMenuEventModel> listevents = JsonConvert.DeserializeObject<IList<Sys_AdminMenuEventModel>>(adminRoleModel.Powers);
                    if (listevents == null || 0 == listevents.Count)
                    {
                        return false;
                    }
                    if (!string.IsNullOrEmpty(menuKey) && !string.IsNullOrEmpty(eventKey))
                    {
                        if (listevents.FirstOrDefault(s => s.EventKey == eventKey && s.MenuKey == menuKey) == null)
                        {
                            //获取用户信息

                            string path = HttpContextHelper.Current.Request.Path;
                            LogExtension.LogInfo($"管理员：{AdminHelper.CurrentUserName}访问地址：{path}没有找到权限！EventKey:{eventKey};MenuKey:{menuKey}");
                            return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                LogExtension.LogWarn(string.Format("判断用户是否具有{0}的访问权限出错，原因：{1}", HttpContextHelper.Current.Request.Path, ex.Message));
                return false;
            }
        }
        #endregion
    }
}

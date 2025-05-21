 
using  System;
using  System.Collections.Generic;
using   System.Linq;
using  System.Threading.Tasks;
using Pandora.Cigfi.Models;
using XCode;
using NewLife.Log;
using Newtonsoft.Json; 
using  Pandora.Cigfi.Models.Sys;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Pandora.Cigfi.Models.Consts.Sys;
using Pandora.Cigfi.Services.Sys;
using FXH.Common.Logger;
using Microsoft.Extensions.DependencyInjection;

namespace Pandora.Cigfi.Web.Common
{
    /// <summary>
    /// 后台授权过滤器
    /// </summary>
    public class MyAuthorizeAttribute : ResultFilterAttribute  
    {
        //private IAdminService _adminService;
        //private IAdminRolesService _adminRolesService;
        /// <summary>
        /// 菜单key
        /// </summary>
        private readonly string _menuKey;
        /// <summary>
        /// 操作key
        /// </summary>
        private readonly string _eventKey;
        /// <summary>
        /// 返回类型 HTML 和 JSON
        /// </summary>
        private readonly string _returnType;
        public MyAuthorizeAttribute( string EventKey, string MenuKey, string returnType =ReturnTypeConsts.HTML)
        {
       
            _menuKey = MenuKey;
            _eventKey = EventKey;
            _returnType = !string.IsNullOrWhiteSpace(returnType) && returnType.ToLower() == ReturnTypeConsts.JSON.ToLower() ? ReturnTypeConsts.JSON : ReturnTypeConsts.HTML;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
             
            IAdminService _adminService = context.HttpContext.RequestServices.GetService<IAdminService>();
            IAdminRolesService _adminRolesService = context.HttpContext.RequestServices.GetService<IAdminRolesService>();
            bool isOK = false;
            var islogin = _adminService.IsLogin().ConfigureAwait(false).GetAwaiter().GetResult();
            if (islogin)
            {
                Sys_AdminModel adminModel =   _adminService.GetAdminInfoAsync(AdminHelper.CurrentUserName).GetAwaiter().GetResult();
                //0528-lhx
                try
                {
                    Sys_AdminRolesModel adminRoleModel = _adminRolesService.GetByIdAsync<int>(adminModel.RoleId).GetAwaiter().GetResult();
                    if (adminRoleModel.IsSuperAdmin == 1)
                    {
                        isOK = true;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(adminRoleModel.Powers))
                        {
                            IList<Sys_AdminMenuEventModel> listevents = JsonConvert.DeserializeObject<IList<Sys_AdminMenuEventModel>>(adminRoleModel.Powers);
                            if (listevents != null && listevents.Count > 0)
                            {
                                if (!string.IsNullOrEmpty(_menuKey) && !string.IsNullOrEmpty(_eventKey))
                                {

                                    if (listevents.FirstOrDefault(s => s.EventKey == _eventKey && s.MenuKey == _menuKey) != null)
                                    {
                                        isOK = true;
                                    }
                                    else
                                    {
                                        LogExtension.LogWarn($"没有找到菜单！EventKey:{_eventKey};MenuKey:{_menuKey}");
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    LogExtension.LogWarn(e.Message);
                }
            }

            //没有权限
            if (!isOK)
            {
                LogExtension.LogWarn($"没有权限菜单！EventKey:{_eventKey};MenuKey:{_menuKey}");
                if (_returnType == ReturnTypeConsts.JSON)
                {
                    JsonTip tip = new JsonTip()
                    {
                        Id = 0,
                        Message = islogin? "您没有权限执行此操作！":"您没有登录或者需要重新登录",
                        ReturnUrl = "/AdminCP/Login"
                    };
                    context.Result = new JsonResult(tip);
                    return;
                }
                else
                {
                    context.Result = new RedirectResult("/AdminCP/Index/NotAuthorize");
                    return;
                }
            }
            base.OnResultExecuting(context);
        }
    }
}

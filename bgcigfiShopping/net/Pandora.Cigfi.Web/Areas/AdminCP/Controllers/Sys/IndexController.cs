using  System;
using  System.Collections.Generic;
using   System.Linq;
using  System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Pandora.Cigfi.Common;
using Pandora.Cigfi.Models;
using XCode;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Pandora.Cigfi.Web.Common;
using NewLife;
using Pandora.Cigfi.Models.Sys; 
using Pandora.Cigfi.Services.Sys; 

namespace Pandora.Cigfi.Web.Areas.AdminCP.Controllers
{
    [Area("AdminCP")]
    public class IndexController : AdminBaseController
    {
        private IAdminRolesService _adminRolesService;
        private IAdminMenuService _adminMenuService;
        
        private readonly IHostingEnvironment _env;
        public IndexController(IHostingEnvironment env, IServiceProvider serviceProvider, IAdminRolesService  adminRolesService, IAdminMenuService adminMenuService) :base(serviceProvider)
        {
            _env = env;
            _adminRolesService= adminRolesService ;
            _adminMenuService = adminMenuService;
        }
        #region 后台首页
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            Sys_AdminModel adminModel= await   _adminService.GetAdminInfoAsync(base.CurrentUserName);
            ViewBag.admin = adminModel??new Sys_AdminViewModel();

            //获取菜单
            List<Sys_AdminMenuViewModel> list = new List<Sys_AdminMenuViewModel>();
            if (adminModel != null)
            { 
                Sys_AdminRolesModel rolesModel = await _adminRolesService.GetByIdAsync<int>(adminModel.RoleId);

                if (rolesModel != null)
                {
                    //这里需要获取权限，暂时先所有
                    if (rolesModel.IsSuperAdmin == 1)
                    {
                        list = await _adminMenuService.GetLeftMenuAsync();
                    }
                    else
                    {
                        List<Sys_AdminMenuModel> menuList = JsonConvert.DeserializeObject<List<Sys_AdminMenuModel>>(rolesModel.Menus);
                        if (menuList == null)
                        {
                            menuList = new List<Sys_AdminMenuModel>();
                        }
                        #region
                        foreach (var item in menuList)
                        {
                            if (0 == item.PId)
                            {
                                Sys_AdminMenuViewModel menuModel = new Sys_AdminMenuViewModel();
                                menuModel.Menu = item;
                                foreach (var subItem in menuList)
                                {
                                    if (subItem.PId == item.Id)
                                    {
                                        menuModel.SubMenuList.Add(subItem);
                                    }
                                }
                                menuModel.SubMenuList.Sort();
                                list.Add(menuModel);
                            }
                        }
                        #endregion
                        list.Sort();
                    }
                }
            }
            return View(list);
        }
        #endregion

        #region 后台主界面
        public IActionResult Main()
        {
            string remoteip = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            string host = Request.HttpContext.Request.Host.Host;
            string port = Request.HttpContext.Connection.RemotePort.ToString();

            ViewBag.remoteip = remoteip;
            ViewBag.port = port;
            ViewBag.host = host;
            ViewBag.contentPath = _env.ContentRootPath;
            ViewBag.rootPath = _env.WebRootPath;

            return View();
        }
        #endregion

        #region 显示没权限
        [AllowAnonymous]
        //显示没有权限
        public IActionResult NotAuthorize()
        {
            return View();
        }
        #endregion
    }
}
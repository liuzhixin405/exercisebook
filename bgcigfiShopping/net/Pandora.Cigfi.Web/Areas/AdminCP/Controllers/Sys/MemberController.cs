using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pandora.Cigfi.Common;
using Pandora.Cigfi.Models;
using XCode;
using Newtonsoft.Json;
using Pandora.Cigfi.Web.Common;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;
using NewLife;

using Pandora.Cigfi.Models.Sys;
using System.Web;
using Pandora.Cigfi.Models.Consts;
using Pandora.Cigfi.Services.Sys;
using FXH.Common.Orm;
using System.Collections;
using Pandora.Cigfi.Models.Consts.Sys;


namespace Pandora.Cigfi.Web.Areas.AdminCP.Controllers
{
    /// <summary>
    /// 后台用户管理控制器
    /// </summary>
    public class MemberController : AdminBaseController
    {
        private readonly IAdminRolesService _adminRoleService;
        private readonly IAdminMenuService _adminMenuService;
        private readonly IAdminMenuEventService _menuEventservice;



        protected readonly Dictionary<int, string> userNames;
        public MemberController(IServiceProvider serviceProvider, IAdminRolesService adminRoleService, IAdminMenuService adminMenuService, IAdminMenuEventService menuEventservice) : base(serviceProvider)
        {
            _adminRoleService = adminRoleService;
            _adminMenuService = adminMenuService;
            _menuEventservice = menuEventservice;
            userNames = _adminService.GetAllAsync().GetAwaiter().GetResult().Where(c=>c.IsLock == 0).ToDictionary(k => k.Id, k => k.RealName);
        }

        protected string GetName(int id)
        {
            if (userNames.ContainsKey(id))
                return userNames[id];
            return "未知用户";
        }

        #region 修改个人信息
        [MyAuthorize(EventKeyConsts.VIEW, MenuKeyConsts.EDITME)]
        public async Task<IActionResult> EditMe()
        {
            var adminModel = await _adminService.GetAdminInfoAsync(base.CurrentUserName);
            await base.WriteLog("查看 修改个人信息", AdminLogType.VIEW, AdminLogLevel.NORMAL);

            return View(adminModel);
        }
        [HttpPost]
        [MyAuthorize(EventKeyConsts.VIEW, MenuKeyConsts.EDITME, ReturnTypeConsts.JSON)]
        public async Task<IActionResult> EditMe(IFormCollection fc)
        {
            var adminModel = await _adminService.GetAdminInfoAsync(base.CurrentUserName);

            string userName = fc["UserName"];
            string oldPwd = fc["txtOldPwd"];
            string newPwd = fc["txtNewPwd"];
            string renewPwd = fc["txtreNewPwd"];
            string realname = fc["RealName"];
            string tel = fc["Tel"];
            string email = fc["Email"];
            string editor = fc["Editor"];
            if (!CommonUtils.IsInt(editor)) editor = "0";
            //判断
            if (string.IsNullOrWhiteSpace(userName))
            {
                tip.Message = "用户名不能为空！";
                return Json(tip);
            }
            userName = userName.Trim();
            if (CommonUtils.GetStringLength(userName) < 5)
            {
                tip.Message = "用户名不能小于5个字符！";
                return Json(tip);
            }
            if (!string.IsNullOrEmpty(email) && !CommonUtils.IsValidEmail(email))
            {
                tip.Message = "请填写正确的Email地址！";
                return Json(tip);
            }

            if (userName != adminModel.UserName)//修改用户名
            {

                if (await _adminService.ExistsAsync(adminModel.Id, userName.Trim()))
                {
                    tip.Message = "新用户名在已经存在，请选择其他用户名！";
                    return Json(tip);
                }
                adminModel.UserName = userName.Trim();
            }

            if (!string.IsNullOrEmpty(newPwd))
            {
                //修改密码的情况
                if (string.IsNullOrWhiteSpace(oldPwd))
                {
                    tip.Message = "您修改密码，旧密码不能为空！";
                    return Json(tip);
                }
                if (newPwd.Length < 5)
                {
                    tip.Message = "新密码不能小于5个字符！";
                    return Json(tip);
                }
                if (newPwd != renewPwd)
                {
                    tip.Message = "您输入的两次密码不一样！";
                    return Json(tip);
                }
                //判断旧密码是否正确
                if (adminModel.PassWord != CommonUtils.MD5(adminModel.Salt + oldPwd.Trim()))
                {
                    tip.Message = "您输入的旧密码不正确，请重新输入！";
                    return Json(tip);
                }
                adminModel.PassWord = CommonUtils.MD5(adminModel.Salt + newPwd);
            }

            adminModel.Tel = tel;
            adminModel.Email = email;
            adminModel.RealName = realname;
            await _adminService.UpdateAsync(adminModel);

            await base.WriteLog("用户信息，修改成功，用户：" + adminModel.UserName, AdminLogType.EDIT, AdminLogLevel.NORMAL);
            tip.Status = JsonTip.SUCCESS;
            tip.Message = "编辑我的信息成功！";
            return Json(tip);
        }
        #endregion

        #region 管理组管理
        [MyAuthorize(EventKeyConsts.VIEWLIST, MenuKeyConsts.ADMINROLE)]
        public async Task<IActionResult> AdminRole()
        {
            // IList<AdminRoles> list = AdminRoles.FindAll(AdminRoles._.Id > 0, AdminRoles._.OrderNo.Asc(), null, 0, 0);
            var list = await _adminRoleService.GetPageListAsync(new Hashtable(), 0, 0);
            await base.WriteLog("查看用户权限列表", AdminLogType.VIEW, AdminLogLevel.NORMAL);
            return View(list.ToList());
        }
        //添加管理组
        [MyAuthorize(EventKeyConsts.ADD, MenuKeyConsts.ADMINROLE)]
        public async Task<IActionResult> AddAdminRole()
        {
            //获取所有的菜单列表

            ViewBag.MenuList = await _adminMenuService.GetListTreeAsync(0, -1, false, false);
            ViewBag.ListEvent = await _menuEventservice.GetMenuEventListAsync();
            await base.WriteLog("查看 添加用户权限组页面", AdminLogType.VIEW, AdminLogLevel.NORMAL);
            return View();
        }
        //执行添加管理组
        [HttpPost]
        [MyAuthorize(EventKeyConsts.ADD, MenuKeyConsts.ADMINROLE, ReturnTypeConsts.JSON)]
        public async Task<IActionResult> AddAdminRole(IFormCollection fc)
        {
            string RoleName = fc["RoleName"];
            string RoleDescription = fc["RoleDescription"];
            string IsSuperAdmin = fc["IsSuperAdmin"];
            string NotAllowDel = fc["NotAllowDel"];
            if (string.IsNullOrEmpty(RoleName))
            {
                tip.Message = "管理组名称不能为空！";
                return Json(tip);
            }
            Sys_AdminRolesModel entity = new Sys_AdminRolesModel();
            entity.RoleName = RoleName;
            entity.RoleDescription = RoleDescription;
            entity.IsSuperAdmin = int.Parse(IsSuperAdmin);
            entity.NotAllowDel = !string.IsNullOrEmpty(NotAllowDel) && NotAllowDel == "1" ? 1 : 0;

            //处理权限
            if (entity.IsSuperAdmin == 1)
            {
                entity.Powers = "";
                entity.Menus = "";
            }
            else
            {
                //第一步，获取菜单
                string[] menuids = Request.Form["menuid"];
                //获取所有的菜单列表
                IList<Sys_AdminMenuModel> alllist = (await _adminMenuService.GetListTreeAsync(0, -1, false, false)).ToList();
                IList<Sys_AdminMenuModel> list = new List<Sys_AdminMenuModel>();
                IList<Sys_AdminMenuEventModel> listevents = new List<Sys_AdminMenuEventModel>();

                var menuEventDc = await _menuEventservice.GetIdMenuEventDcAsync();
                if (menuids != null && menuids.Length > 0)
                {
                    foreach (string s in menuids)
                    {
                        if (CommonUtils.IsInt(s) && alllist.FirstOrDefault(v => v.Id == int.Parse(s)) != null)
                        {
                            Sys_AdminMenuModel tmp = alllist.FirstOrDefault(a => a.Id == int.Parse(s));
                            list.Add(tmp);
                            //处理详细权限  详细权限，每一行，则每一个菜单的详细权限，则同一个name
                            string[] eventids = Request.Form["EventKey_" + s];
                            if (eventids != null && eventids.Length > 0)
                            {
                                foreach (var item in eventids)
                                {
                                    if (CommonUtils.IsInt(item))
                                    {
                                        if (menuEventDc.ContainsKey(int.Parse(item)))
                                        {

                                            listevents.Add(menuEventDc[int.Parse(item)]);
                                        }

                                    }
                                }
                            }
                        }
                    }
                    //序列化成json
                    if (list != null && list.Count > 0)
                    {
                        entity.Menus = JsonConvert.SerializeObject(list);
                    }
                    if (listevents != null && listevents.Count > 0)
                    {
                        entity.Powers = JsonConvert.SerializeObject(listevents);
                    }
                }
            }

            await _adminRoleService.InsertAsync(entity);
            tip.Status = JsonTip.SUCCESS;
            tip.Message = "添加管理组成功";
            tip.ReturnUrl =JsonReturnConsts.CLOSE;
            await base.WriteLog($"添加新管理组，成功，名称：" + entity.RoleName, AdminLogType.ADD, AdminLogLevel.NORMAL);
            return Json(tip);
        }

        //查看编辑管理组
        [MyAuthorize(EventKeyConsts.EDIT, MenuKeyConsts.ADMINROLE)]
        public async Task<IActionResult> EditAdminRole(int id)
        {
            Sys_AdminRolesModel entity = await _adminRoleService.GetByIdAsync<int>(id);
            if (entity == null)
            {
                return EchoTipPage("系统找不到本记录！", 0, true, "");
            }
            if (string.IsNullOrEmpty(entity.Powers))
                entity.Powers = "[]";
            if (string.IsNullOrEmpty(entity.Menus))
                entity.Menus = "[]";
            //获取所有的菜单列表
            // IList<AdminMenu> list = AdminMenu.GetListTree(0, -1, false, false);
            ViewBag.MenuList = await _adminMenuService.GetListTreeAsync(0, -1, false, false);
            ViewBag.ListEvent = await _menuEventservice.GetMenuEventListAsync();
            await base.WriteLog($"查看管理组详情，管理组名称：" + entity.RoleName, AdminLogType.VIEW, AdminLogLevel.NORMAL);
            return View(entity);
        }


        //执行编辑管理组
        [HttpPost]
        [MyAuthorize(EventKeyConsts.EDIT, MenuKeyConsts.ADMINROLE, ReturnTypeConsts.JSON)]
        public async Task<IActionResult> EditAdminRole(IFormCollection fc)
        {
            string Id = fc["Id"];
            string RoleName = fc["RoleName"];
            string RoleDescription = fc["RoleDescription"];
            string IsSuperAdmin = fc["IsSuperAdmin"];
            string NotAllowDel = fc["NotAllowDel"];
            if (string.IsNullOrEmpty(Id))
            {
                tip.Message = "错误参数传递！";
                return Json(tip);
            }

            if (string.IsNullOrEmpty(RoleName))
            {
                tip.Message = "管理组名称不能为空！";
                return Json(tip);
            }
            Sys_AdminRolesModel entity = await _adminRoleService.GetByIdAsync<int>(int.Parse(Id));
            if (entity == null)
            {
                return EchoTipPage("系统找不到本记录！", 0, true, "");
            }

            entity.RoleName = RoleName;
            entity.RoleDescription = RoleDescription;
            entity.IsSuperAdmin = int.Parse(IsSuperAdmin);
            entity.NotAllowDel = !string.IsNullOrEmpty(NotAllowDel) && NotAllowDel == "1" ? 1 : 0;

            //处理权限
            if (entity.IsSuperAdmin == 1)
            {
                entity.Powers = "";
                entity.Menus = "";
            }
            else
            {
                //第一步，获取菜单
                string[] menuids = Request.Form["menuid"];
                //获取所有的菜单列表
                IList<Sys_AdminMenuModel> alllist = (await _adminMenuService.GetListTreeAsync(0, -1, false, false)).ToList();
                IList<Sys_AdminMenuModel> list = new List<Sys_AdminMenuModel>();
                IList<Sys_AdminMenuEventModel> listevents = new List<Sys_AdminMenuEventModel>();

                var menuEventDc = await _menuEventservice.GetIdMenuEventDcAsync();
                if (menuids != null && menuids.Length > 0)
                {
                    foreach (string s in menuids)
                    {
                        if (CommonUtils.IsInt(s) && alllist.FirstOrDefault(v => v.Id == int.Parse(s)) != null)
                        {
                            Sys_AdminMenuModel tmp = alllist.FirstOrDefault(a => a.Id == int.Parse(s));
                            list.Add(tmp);
                            //处理详细权限  详细权限，每一行，则每一个菜单的详细权限，则同一个name
                            string[] eventids = Request.Form["EventKey_" + s];
                            if (eventids != null && eventids.Length > 0)
                            {
                                foreach (var item in eventids)
                                {
                                    if (CommonUtils.IsInt(item))
                                    {
                                        if (menuEventDc.ContainsKey(int.Parse(item)))
                                        {
                                            listevents.Add(menuEventDc[int.Parse(item)]);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    //序列化成json
                    if (list != null && list.Count > 0)
                    {
                        entity.Menus = JsonConvert.SerializeObject(list);
                    }
                    if (listevents != null && listevents.Count > 0)
                    {
                        entity.Powers = JsonConvert.SerializeObject(listevents);
                    }
                }
            }
            await _adminRoleService.UpdateAsync(entity);
            tip.Status = JsonTip.SUCCESS;
            tip.Message = "编辑管理组详情成功";
            tip.ReturnUrl =JsonReturnConsts.CLOSE;
            await base.WriteLog($"修改管理组，成功，名称：" + entity.RoleName, AdminLogType.EDIT, AdminLogLevel.NORMAL);
            return Json(tip);
        }
        //删除管理组
        [HttpPost]
        [MyAuthorize(EventKeyConsts.DEL, MenuKeyConsts.ADMINROLE, ReturnTypeConsts.JSON)]
        public async Task<IActionResult> DelAdminRole(int id)
        {
            Sys_AdminRolesModel entity = await _adminRoleService.GetByIdAsync<int>(id);
            if (entity == null)
            {
                tip.Message = "系统找不到本管理组详情！";
                return Json(tip);
            }

            if (entity.NotAllowDel == 1)
            {
                tip.Message = "本管理组设定不允许删除，如果需要删除，请先解除限制！";
                return Json(tip);
            }
            //如果不是超级管理员，不允许删除
            Sys_AdminModel adminModel = await _adminService.GetAdminInfoAsync(base.CurrentUserName);
            Sys_AdminRolesModel roleModel = await _adminRoleService.GetByIdAsync<int>(adminModel.RoleId);
            if (roleModel.IsSuperAdmin != 1)
            {
                tip.Message = "非超级管理员，不能执行此操作！";
                return Json(tip);
            }
            //如果只有一个管理组，不允许删除！
            if (await _adminRoleService.CountAsync(new Hashtable()) <= 1)
            {
                tip.Message = "只有一个管理组，不能删除！";
                return Json(tip);
            }
            //删除管理组，并删除旗下所有管理员 
            await _adminRoleService.DeleteAsync(entity);
            tip.Status = JsonTip.SUCCESS;
            tip.Message = "删除管理组成功";
            await base.WriteLog($"删除管理组，名称: {entity.RoleName}", AdminLogType.DEL, AdminLogLevel.NORMAL);
            return Json(tip);
        }
        #endregion

        #region 管理员管理
        //管理员管理
        [MyAuthorize(EventKeyConsts.VIEWLIST, MenuKeyConsts.ADMIN)]
        public async Task<IActionResult> Admins()
        {


            ViewBag.RoleList = await _adminRoleService.GetPageListAsync(new Hashtable(), 0, 0);
            await base.WriteLog("查看管理员列表", AdminLogType.VIEW, AdminLogLevel.NORMAL);
            return View();
        }
        [MyAuthorize(EventKeyConsts.VIEWLIST, MenuKeyConsts.ADMIN, ReturnTypeConsts.JSON)]
        public async Task<IActionResult> GetAdmins(string keyword, int page = 1, int limit = 20, int roleid = 0)
        {

            var pageResult = new PagedResult<Sys_AdminModel>();
            Hashtable queryHt = new Hashtable();
            if (!string.IsNullOrEmpty(keyword))
            {
                queryHt.Add("keyword", keyword);
            }
            if (roleid > 0)
            {
                queryHt.Add("roleid", roleid);
            }


            pageResult.rows = await _adminService.GetPageListAsync(queryHt, page, limit);
            pageResult.total = await _adminService.CountAsync(queryHt);

            return Content(JsonConvert.SerializeObject(pageResult), "text/plain");
            //  return Content(JsonConvert.SerializeObject(new { total = totalCount, rows = list }), "text/plain");
        }
        //添加管理员
        [MyAuthorize(EventKeyConsts.ADD, MenuKeyConsts.ADMIN)]
        public async Task<IActionResult> AddAdmin()
        {

            ViewBag.RoleList = await _adminRoleService.GetPageListAsync(new Hashtable(), 0, 0);
            await base.WriteLog("查看添加管理员页面", AdminLogType.VIEW, AdminLogLevel.NORMAL);
            return View();
        }
        //执行添加管理员
        [HttpPost]
        [MyAuthorize(EventKeyConsts.ADD, MenuKeyConsts.ADMIN, ReturnTypeConsts.JSON)]
        public async Task<IActionResult> AddAdmin(IFormCollection fc)
        {
            string userName = fc["UserName"];
            string phoneNo = fc["Tel"];
            string roleid = fc["RoleId"];
            string realname = fc["RealName"];
            string newPwd = fc["PassWord"];
            string renewPwd = fc["PassWord2"];
            if (!CommonUtils.IsInt(roleid))
            {
                tip.Message = "请选择一个管理组！";
                return Json(tip);
            }

            if (string.IsNullOrEmpty(userName))
            {
                tip.Message = "登录用户名不能为空！";
                return Json(tip);
            }
            if (CommonUtils.GetStringLength(userName.Trim()) < 5)
            {
                tip.Message = "登录用户名不能小于5个字节！";
                return Json(tip);
            }
            //if (!MobileNoHelper.IsVaildPhoneNumber("86",phoneNo.Trim()))
            //{
            //    tip.Message = "无效号码";
            //    return Json(tip);
            //}
            if (string.IsNullOrEmpty(newPwd))
            {
                tip.Message = "密码不能为空！";
                return Json(tip);
            }
            if (newPwd.Length < 5)
            {
                tip.Message = "密码不能小于5个字符！";
                return Json(tip);
            }
            if (newPwd != renewPwd)
            {
                tip.Message = "两次输入密码不一致，请重新输入！";
                return Json(tip);
            }
            //验证用户名
            if (await _adminService.ExistsAsync(0, userName))
            {
                tip.Message = "该用户名已经存在，请选择其他用户名！";
                return Json(tip);
            }

            Sys_AdminModel adminModel = new Sys_AdminModel();
            adminModel.UserName = userName;
            adminModel.RealName = realname;
            adminModel.Salt = CommonUtils.GetRandomChar(10);
            adminModel.PassWord = CommonUtils.MD5(adminModel.Salt + newPwd);
            adminModel.RoleId = int.Parse(roleid);
            adminModel.Tel = phoneNo;
            await _adminService.InsertAsync(adminModel);

            tip.Status = JsonTip.SUCCESS;
            tip.Message = "添加后台人员成功！";
            tip.ReturnUrl =JsonReturnConsts.CLOSE;
            await base.WriteLog($"添加后台人员，名称：{adminModel.UserName}", AdminLogType.ADD, AdminLogLevel.NORMAL);
            return Json(tip);
        }

        //查看，编辑管理员
        [MyAuthorize(EventKeyConsts.EDIT, MenuKeyConsts.ADMIN)]
        public async Task<IActionResult> EditAdmin(int id)
        {
            ViewBag.RoleList = await _adminRoleService.GetPageListAsync(new Hashtable(), 0, 0);
            Sys_AdminModel entity = new Sys_AdminModel();
            if (id > 0)
            {
                entity = await _adminService.GetByIdAsync<int>(id);
            }

            if (entity == null)
            {
                return EchoTipPage("系统找不到本记录！");
            }
            await base.WriteLog($"查看/编辑管理员({entity.UserName});", AdminLogType.VIEW, AdminLogLevel.NORMAL);

            return View(entity);
        }
        //执行编辑管理员
        [HttpPost]
        [MyAuthorize(EventKeyConsts.EDIT, MenuKeyConsts.ADMIN, ReturnTypeConsts.JSON)]
        public async Task<IActionResult> EditAdmin(IFormCollection fc)
        {
            string userName = fc["UserName"];
            string roleid = fc["RoleId"];
            string realname = fc["RealName"];
            string newPwd = fc["PassWord"];
            string renewPwd = fc["PassWord2"];
            string Id = fc["Id"];
            string phoneNo = fc["Tel"];

            if (!CommonUtils.IsInt(Id))
            {
                tip.Message = "错误参数传递！";
                return Json(tip);
            }
            Sys_AdminModel entity = new Sys_AdminModel();
            entity = await _adminService.GetByIdAsync<int>(Convert.ToInt32(Id));
            if (entity == null)
            {
                tip.Message = "系统找不到本记录！";
                return Json(tip);
            }
            if (!CommonUtils.IsInt(roleid))
            {
                tip.Message = "请选择一个管理组！";
                return Json(tip);
            }

            if (string.IsNullOrEmpty(userName))
            {
                tip.Message = "登录用户名不能为空！";
                return Json(tip);
            }
            //if (!MobileNoHelper.IsVaildPhoneNumber("86", phoneNo.Trim()))
            //{
            //    tip.Message = "无效号码";
            //    return Json(tip);
            //}
            if (CommonUtils.GetStringLength(userName.Trim()) < 5)
            {
                tip.Message = "登录用户名不能小于5个字节！";
                return Json(tip);
            }
            if (entity.UserName != userName)//修改用户名
            {

                //验证用户名是否存在
                if (await _adminService.ExistsAsync(entity.Id, userName))
                {
                    tip.Message = "该用户名已经存在，请选择其他用户名！";
                    return Json(tip);
                }
                entity.UserName = userName;
            }
            if (!string.IsNullOrEmpty(newPwd))//修改密码
            {
                if (newPwd.Length < 5)
                {
                    tip.Message = "密码不能小于5个字符！";
                    return Json(tip);
                }
                if (newPwd != renewPwd)
                {
                    tip.Message = "两次输入密码不一致，请重新输入！";
                    return Json(tip);
                }
                entity.PassWord = CommonUtils.MD5(entity.Salt + newPwd);
            }
            entity.RoleId = int.Parse(roleid);
            entity.RealName = realname;
            entity.Tel = phoneNo.Trim();
            await _adminService.UpdateAsync(entity);

            tip.Status = JsonTip.SUCCESS;
            tip.Message = "修改后台人员信息成功！";
            tip.ReturnUrl =JsonReturnConsts.CLOSE;
            await base.WriteLog($"修改后台人员信息({entity.UserName});", AdminLogType.EDIT, AdminLogLevel.NORMAL);
            return Json(tip);
        }
        //删除管理员
        [HttpPost]
        [MyAuthorize(EventKeyConsts.DEL, MenuKeyConsts.ADMIN, ReturnTypeConsts.JSON)]
        public async Task<IActionResult> DelAdmin(int id)
        {
            Sys_AdminModel entity = await _adminService.GetByIdAsync<int>(id);

            if (entity == null)
            {
                tip.Message = "系统找不到本记录！";
                return Json(tip);
            }

            if (entity.Id.ToString().Equals(base.CurrentUserID))
            {
                tip.Message = "您不可以删除自己！";
                return Json(tip);
            }
            Sys_AdminModel curUserModel = await _adminService.GetByIdAsync<int>(int.Parse(base.CurrentUserID));
            Sys_AdminRolesModel roleAdmin = await _adminRoleService.GetByIdAsync<int>(curUserModel.RoleId);
            //非超级管理员不能删除用户
            if (roleAdmin.IsSuperAdmin != 1)
            {
                tip.Message = "您不是超级管理员，不能删除用户！";
                return Json(tip);
            }
            await _adminService.DeleteAsync(new Sys_AdminModel() { Id = id });
            await base.WriteLog($"删除管理员({entity.UserName});", AdminLogType.DEL, AdminLogLevel.NORMAL);

            tip.Status = JsonTip.SUCCESS;
            tip.Message = "删除管理员成功";
            return Json(tip);
        }
        #endregion

        #region 审核概览
        public IActionResult ReviewCount()
        {
            return View();
        }

        [MyAuthorize(EventKeyConsts.VIEWLIST, MenuKeyConsts.REVIEWCOUNT)]
        public async Task<IActionResult> GetPageRCListAsync(int page = 1, int limit = 20)
        {
            var pageResult = new PagedResult<ReviewCountModel>();
            
           
          
            pageResult.total = userNames.Count+1;
            var result = new List<ReviewCountModel>(pageResult.total);
            //将用户列表赋值给result
            foreach (var key in userNames.Keys)
            {
                var item = new ReviewCountModel {ID = key, AddOprName = GetName(key)};
                result.Add(item);
            }
            //用户ID等于0为未知用户
            result.Add(new ReviewCountModel { ID = 0, AddOprName = GetName(0) });
            //根据用户列表进行数值统计
            foreach (var reviewCountModel in result)
            {
              
                //当前周几减去周一
                var diffWeek = DateTime.Now.DayOfWeek - DayOfWeek.Monday;
                //上周
                if (diffWeek < 0)
                    diffWeek += 7;
                var startWeek = DateTime.Now.AddDays(-diffWeek).Date;
                var endWeek = startWeek.AddDays(6).Date;
             

                var startDay = DateTime.Today;
                var endDay = startDay.AddDays(1).AddSeconds(-1);
               
            }
            pageResult.rows = result;
            return Content(JsonConvert.SerializeObject(pageResult), "text/plain");
        }
        #endregion

        #region 后台管理日志列表
        [MyAuthorize(EventKeyConsts.VIEWLIST, MenuKeyConsts.ADMINLOG)]
        public async Task<IActionResult> AdminCPLogList()
        {
            await Task.Delay(10);
            return View();
        }
        [MyAuthorize(EventKeyConsts.VIEWLIST, MenuKeyConsts.ADMINLOG)]
        public async Task<IActionResult> GetAdminCPLogList(string logip, string userName, string keyword, string beginTime, string endTime, int page = 1, int limit = 20)
        {
            var pageResult = new PagedResult<Sys_AdminLogModel>();

            Hashtable queryHt = new Hashtable();
            if (!string.IsNullOrEmpty(keyword))
            {
                queryHt.Add("keyword", keyword);
            }
            if (!string.IsNullOrEmpty(logip))
            {
                queryHt.Add("logip", logip);
            }
            if (!string.IsNullOrEmpty(userName))
            {
                queryHt.Add("userName", userName);
            }
            if (!string.IsNullOrEmpty(beginTime))
            {
                queryHt.Add("beginTime", Convert.ToDateTime(beginTime).ToString("yyyy-MM-dd"));
            }
            if (!string.IsNullOrEmpty(endTime))
            {
                queryHt.Add("endTime", Convert.ToDateTime(endTime).ToString("yyyy-MM-dd"));
            }

            pageResult.rows = await _adminLogService.GetPageListAsync(queryHt, page, limit);
            pageResult.total = await _adminLogService.CountAsync(queryHt);

            return Content(JsonConvert.SerializeObject(pageResult), "text/plain");

            //int numPerPage, currentPage, startRowIndex;

            //numPerPage = limit;
            //currentPage = page;
            //startRowIndex = (currentPage - 1) * numPerPage;
            //Expression ex = AdminLog._.Id > 0;

            //if (!string.IsNullOrWhiteSpace(keyword))
            //{
            //    if (CommonUtils.IsInt(keyword))
            //    {
            //        ex &= (AdminLog._.Id == int.Parse(keyword) | AdminLog._.UserName.Contains(keyword));
            //    }
            //    else
            //    {
            //        ex &= AdminLog._.UserName.Contains(keyword);
            //    }
            //}
            //string kid = Request.Query["kid"];

            //IList<AdminLog> list = AdminLog.FindAll(ex, AdminLog._.Id.Desc(), null, startRowIndex, numPerPage);
            //long totalCount = AdminLog.FindCount(ex, AdminLog._.Id.Desc(), null, startRowIndex, numPerPage);
            //Admin.WriteLogActions("后台管理日志留言列表(page:" + page + ");");
            //return Content(Newtonsoft.Json.JsonConvert.SerializeObject(new { total = totalCount, rows = list }), "text/plain");
        }

        #endregion

        #region 查看管理日志详情
        [MyAuthorize(EventKeyConsts.VIEW, MenuKeyConsts.ADMINLOG)]
        public async Task<IActionResult> ViewAdminLogDetail(int id)
        {
            Sys_AdminLogModel entity = await _adminLogService.GetByIdAsync<int>(id);
            if (entity == null)
            {
                return EchoTipPage("系统找不到本记录！");
            }
            await base.WriteLog("查看管理日志详情，日志id：" + id, AdminLogType.DEL, AdminLogLevel.NORMAL);
            return View(entity);
        }
        #endregion
    }
}
using  System;
using  System.Collections.Generic;
using   System.Linq;
using  System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pandora.Cigfi.Common;
using Pandora.Cigfi.Models;
using XCode;
using Newtonsoft.Json;
using Pandora.Cigfi.Web.Common;
using Microsoft.AspNetCore.Http;
using  Pandora.Cigfi.Models.Sys;
using NewLife;

using Pandora.Cigfi.Models.Consts;
using Pandora.Cigfi.Services.Sys;
using System.Collections;
using FXH.Common.Orm;
using Pandora.Cigfi.Models.Consts.Sys;

namespace Pandora.Cigfi.Web.Areas.AdminCP.Controllers
{
    [Area("AdminCP")]
    public class PermissionController : AdminBaseController
    {
        private ITargetEventService _dataservice;
        private IAdminMenuService _adminMenuservice;
        private IAdminMenuEventService _menuEventservice;
        public PermissionController(IServiceProvider serviceProvider, ITargetEventService dataservice, IAdminMenuService adminMenuservice, IAdminMenuEventService menuEventservice) : base(serviceProvider)
        {
            _dataservice = dataservice;
            _adminMenuservice = adminMenuservice;
            _menuEventservice = menuEventservice;
        }
        #region 事件管理
        /// <summary>
        /// 目标事件
        /// </summary>
        /// <returns></returns>
        [MyAuthorize(EventKeyConsts.VIEW, MenuKeyConsts.EVENTKEY)]
        public async Task<IActionResult> EventKey()
        {
            await base.WriteLog("查看事件权限", AdminLogType.VIEW, AdminLogLevel.NORMAL);
            return View( );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [MyAuthorize(EventKeyConsts.VIEWLIST,  MenuKeyConsts.EVENTKEY, ReturnTypeConsts.JSON)]
        public async Task<IActionResult> GetEventKey(string keyword, int page = 1, int limit = 20)
        {
            var pageResult = new PagedResult<Sys_TargetEventModel>();

            Hashtable queryHt = new Hashtable();
            if (!string.IsNullOrEmpty(keyword))
            {
                queryHt.Add("keyword", keyword);
            }
            if(page<1)
            {
                page = 1;
            }
            pageResult.rows = await _dataservice.GetPageListAsync(queryHt, page, limit);
            pageResult.total = await _dataservice.CountAsync(queryHt); 
            await base.WriteLog($"查看后台事件权限列表（keyword:{keyword};page:{page};limit:{limit}）;", AdminLogType.VIEW, AdminLogLevel.NORMAL);
            return Content(JsonConvert.SerializeObject(pageResult), "text/plain"); 
        }

        //添加eventkey
        [MyAuthorize(EventKeyConsts.ADD,  MenuKeyConsts.EVENTKEY)]
        public async Task<IActionResult> AddEventKey()
        {
            await base.WriteLog($"查看添加后台事件页面", AdminLogType.VIEW, AdminLogLevel.NORMAL); 
            return View();
        }

        [MyAuthorize(EventKeyConsts.ADD,  MenuKeyConsts.EVENTKEY,  ReturnTypeConsts.JSON)]
        [HttpPost]
        public async Task<IActionResult> AddEventKey(IFormCollection fc)
        {
            Sys_TargetEventModel entity = new Sys_TargetEventModel();
            entity.EventKey = fc["EventKey"];
            entity.EventName = fc["EventName"];
            string rank = fc["OrderNo"];
            if (string.IsNullOrWhiteSpace(rank) || !CommonUtils.IsInt(rank)) rank = "0";
            entity.OrderNo = int.Parse(rank);
            if (string.IsNullOrWhiteSpace(fc["IsDisable"]) || fc["IsDisable"] != "1")
                entity.IsDisable = 0;
            else
                entity.IsDisable = 1;

            if (string.IsNullOrWhiteSpace(entity.EventKey))
            {
                tip.Message = "请填写事件KEY";
                return Json(tip);
            }
            if (string.IsNullOrWhiteSpace(entity.EventName))
            {
                tip.Message = "请填写事件名称";
                return Json(tip);
            }
            entity.EventKey = entity.EventKey.ToLower();

            if (await _dataservice.ExistsAsync(entity.EventKey.ToLower()))
            {
                tip.Message = "事件key已经存在，请重新填写一个";
                return Json(tip);
            }

            await   _dataservice.InsertAsync(entity);
            tip.Status = JsonTip.SUCCESS;
            tip.Message = "添加事件成功";
            tip.ReturnUrl =JsonReturnConsts.CLOSE;

             
            await base.WriteLog("添加事件详情(id:" + entity.Id + ")；", AdminLogType.ADD, AdminLogLevel.NORMAL);
            return Json(tip);
        }
        #endregion

        #region 后台栏目管理
        //后台菜单列表
        [MyAuthorize(EventKeyConsts.VIEWLIST, MenuKeyConsts.ADMINMENU)]
        public async Task<IActionResult> AdminMenuList()
        {
         //  IList<AdminMenu> list = AdminMenu.GetListTree(0, -1, false, true);
           var list=await _adminMenuservice.GetListTreeAsync(0, -1, true, false);
          
            ViewBag.ListEvent = await _menuEventservice.GetMenuEventListAsync();
            
            await base.WriteLog("查看后台栏目管理", AdminLogType.VIEW, AdminLogLevel.NORMAL);
            return View(list);
        }
        //添加后台菜单
        [MyAuthorize(EventKeyConsts.ADD,  MenuKeyConsts.ADMINMENU)]
        public async Task<IActionResult> AddAdminMenu()
        {
          var leftTree=  await  _adminMenuservice.GetListTreeAsync(0, -1, true, false);
            //获取上级栏目
         // IList<AdminMenu> list = AdminMenu.GetListTree(0, -1, true, false);
            ViewBag.ListTree = leftTree.FindAll(x=>x.PId==0);
            ViewBag.ListEvent = await _dataservice.GetAllAsync(); 
            return View();
        }
        //执行添加菜单
        [HttpPost]
        [MyAuthorize(EventKeyConsts.ADD,  MenuKeyConsts.ADMINMENU,  ReturnTypeConsts.JSON)]
        public async Task<IActionResult> AddAdminMenu(Sys_AdminMenuModel model)
        {
            if (!ModelState.IsValid)
            {
                string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

                tip.Message = messages;
                return Json(tip);
            }
           
            //判断
            if (await _adminMenuservice.ExistsAsync(model.MenuKey))
            {
                tip.Message = "菜单KEY已经存在，请填写其他的！";
                return Json(tip);
            }
            IList<Sys_TargetEventModel> listevent =( await _dataservice.GetAllAsync()).ToList();

            string[] eventkeys = Request.Form[MenuKeyConsts.EVENTKEY];
            List<Sys_AdminMenuEventModel> menuEventList = new List<Sys_AdminMenuEventModel>();
            if (eventkeys != null && eventkeys.Length > 0)
            {
                foreach (string s in eventkeys)
                {
                    if (CommonUtils.IsInt(s))
                    {
                        Sys_TargetEventModel te = listevent.FirstOrDefault(t => t.Id == int.Parse(s));
                        if (te != null)
                        {
                            Sys_AdminMenuEventModel tmp = new Sys_AdminMenuEventModel();
                            tmp.EventId = te.Id;
                            tmp.EventKey = te.EventKey;
                            tmp.EventName = te.EventName;
                            tmp.MenuKey = model.MenuKey; 
                            menuEventList.Add(tmp);
                        }
                    }
                }
            }

          await   _adminMenuservice.InsertMenuAsync(model, menuEventList);
         
            tip.Status = JsonTip.SUCCESS; 
            tip.ReturnUrl =JsonReturnConsts.CLOSE;
            tip.Message = "添加成功";
            await base.WriteLog("添加后台菜单成功", AdminLogType.ADD, AdminLogLevel.NORMAL);
            return Json(tip);
        }

        //修改菜单
        public async Task<IActionResult> EditAdminMenu(int id)
        {
           Sys_AdminMenuModel menuModel= await _adminMenuservice.GetByIdAsync(id); 
            if (menuModel == null)
            {
                return EchoTipPage("系统找不到本记录！");
            }
           
            ViewBag.ListTree = await _adminMenuservice.GetListTreeAsync(0, -1, true, false); ; 
            ViewBag.ListEvent = await _dataservice.GetAllAsync();
            ViewBag.CurMenuEvent = await   _menuEventservice.GetMenuEventListAsync(id);

            return View(menuModel);
        }

        //执行添加菜单
        [HttpPost]
        [MyAuthorize(EventKeyConsts.EDIT,  MenuKeyConsts.ADMINMENU,  ReturnTypeConsts.JSON)]
        public async Task<IActionResult> EditAdminMenu(Sys_AdminMenuModel model)
        {
            if (!ModelState.IsValid)
            {
                string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                tip.Message = messages;
                return Json(tip);
            }
            if (model.Id <= 0)
            {
                tip.Message = "错误参数传递！";
                return Json(tip);
            }
            Sys_AdminMenuModel menuModel = await _adminMenuservice.GetByIdAsync(model.Id);
            if (menuModel == null)
            {
                return EchoTipPage("系统找不到本记录！");
            }

            //赋值
            menuModel.MenuName = model.MenuName;
            //如果key修改了。判断是否有存在
            if (menuModel.MenuKey != model.MenuKey)
            {
                
                if (await _adminMenuservice.ExistsAsync(model.MenuKey, menuModel.Id))
                {
                    tip.Message = "您修改的菜单KEY已经存在，请填写其他的！";
                    return Json(tip);
                }
                menuModel.MenuKey = model.MenuKey;
            }
            menuModel.Link = model.Link;
            menuModel.IsHide = model.IsHide;
            menuModel.ClassName = model.ClassName;
            menuModel.OrderNo = model.OrderNo;
            if (menuModel.PId != model.PId)
            {
                if(model.PId>0)
                {
                    Sys_AdminMenuModel parentMenuModel = await _adminMenuservice.GetByIdAsync(model.PId);
                    menuModel.Level = parentMenuModel.Level + 1;
                    menuModel.Location = parentMenuModel.Location+ parentMenuModel.Id+",";

                }
                else
                {
                    menuModel.Level = 0;
                    menuModel.Location = "0,";
                } 
            }
            menuModel.PId = model.PId;

        
            IList<Sys_TargetEventModel> listevent = (await _dataservice.GetAllAsync()).ToList();

            string[] eventkeys = Request.Form[MenuKeyConsts.EVENTKEY];
            List<Sys_AdminMenuEventModel> menuEventList = new List<Sys_AdminMenuEventModel>();
            if (eventkeys != null && eventkeys.Length > 0)
            {
                foreach (string s in eventkeys)
                {
                    if (CommonUtils.IsInt(s))
                    {
                        Sys_TargetEventModel te = listevent.FirstOrDefault(t => t.Id == int.Parse(s));
                        if (te != null)
                        {
                            Sys_AdminMenuEventModel tmp = new Sys_AdminMenuEventModel();
                            tmp.EventId = te.Id;
                            tmp.EventKey = te.EventKey;
                            tmp.EventName = te.EventName;
                            tmp.MenuKey = model.MenuKey;
                            tmp.MenuId = menuModel.Id;
                            menuEventList.Add(tmp);
                        }
                    }
                }
            }
            await _adminMenuservice.UpdateMenuAsync(menuModel, menuEventList); 

            tip.Status = JsonTip.SUCCESS;
            tip.Message = "编辑后台菜单成功";
            tip.ReturnUrl =JsonReturnConsts.CLOSE;
            await base.WriteLog("编辑后台菜单成功", AdminLogType.ADD, AdminLogLevel.NORMAL);
            return Json(tip);
        }

        //删除菜单
        [HttpPost]
        [MyAuthorize(EventKeyConsts.DEL,  MenuKeyConsts.ADMINMENU,  ReturnTypeConsts.JSON)]
        public async Task<IActionResult> DelAdminMenu(int id)
        {
            Sys_AdminMenuModel menuModel = await _adminMenuservice.GetByIdAsync(id);
            if (menuModel == null)
            {
                return EchoTipPage("系统找不到本记录！");
            }
            Hashtable whereHt = new Hashtable();
            whereHt.Add("pid", menuModel.Id);
            if (await _adminMenuservice.CountAsync(whereHt)>0)
            {
                tip.Message = "本菜单有下级菜单，不允许删除！";
                return Json(tip);
            }
          await   _adminMenuservice.DeleteAsync(menuModel);
            tip.Status = JsonTip.SUCCESS;
            tip.Message = "删除后台菜单成功";
            await base.WriteLog("删除后台菜单成功", AdminLogType.DEL, AdminLogLevel.NORMAL); 
            return Json(tip);
        }
        #endregion
    }
}
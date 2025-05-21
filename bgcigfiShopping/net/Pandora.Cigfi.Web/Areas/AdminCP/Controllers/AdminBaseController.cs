using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pandora.Cigfi.Common;
using Pandora.Cigfi.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Pandora.Cigfi.Models.Sys;
using System.Text;
using FXH.Common.Logger;
using FXH.Web.Extensions.Http;
using Pandora.Cigfi.Models.Consts.Sys;
using Pandora.Cigfi.Services.Sys; 
using static Pandora.Cigfi.Models.Sys.Sys_ReviewLogModel;
using Pandora.Cigfi.IServices.Sys;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection; 
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Pandora.Cigfi.Web.Areas.AdminCP.Controllers
{
    [Area("AdminCP")]
    public class AdminBaseController : Controller
    {
        
        public IServiceProvider _serviceProvider;
        /// <summary>
        ///用户服务接口
        /// </summary>
        public IAdminService _adminService;
        /// <summary>
        /// 用户的日志接口
        /// </summary>
        public IAdminLogService _adminLogService;
        //public IAdminRolesService _adminRolesService;
        private readonly IReviewLogService _reviewLogService;

        public AdminBaseController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _reviewLogService = _serviceProvider.GetService<IReviewLogService>();
            _adminService = _serviceProvider.GetService<IAdminService>();
            _adminLogService = _serviceProvider.GetService<IAdminLogService>();
            // _adminRolesService = _serviceProvider.GetService<IAdminRolesService>();
        }
        public JsonTip tip = new JsonTip();
        /// <summary>
        /// 当前登录用户的ID
        /// </summary>
        public string CurrentUserID
        {
            get
            {
                return AdminHelper.CurrentUserID;
            }
        }

        /// <summary>
        /// 当前登录用户的帐号
        /// </summary>
        public string CurrentUserName
        {
            get
            {
                return AdminHelper.CurrentUserName;
            }
        }




        /// <summary>
        /// 模型错误信息
        /// </summary>
        /// <returns></returns>
        public string ModelStateError()
        {

            StringBuilder errinfo = new StringBuilder();
            foreach (var s in ModelState.Values)
            {
                foreach (var p in s.Errors)
                {
                    errinfo.AppendFormat("{0}\\n", p.ErrorMessage);
                }
            }
            return errinfo.ToString();
        }
        
        /// <summary>
        /// 如果没登录
        /// </summary>
        /// <param name="context"></param>
        public async override void OnActionExecuting(ActionExecutingContext context)
        {


            bool islogin = await _adminService.IsLogin();

            if (!islogin)
            {
                context.Result = new RedirectResult("/AdminCP/Login");
                return;
            }
            base.OnActionExecuting(context);
        }

        #region 获取验证第一条错误
        /// <summary>
        /// 获取服务端验证的第一条错误信息
        /// </summary>
        /// <returns></returns>
        public string GetModelStateError()
        {
            foreach (var item in ModelState.Values)
            {
                if (item.Errors.Count > 0)
                {
                    return item.Errors[0].ErrorMessage;
                }
            }
            return "";
        }
        #endregion

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                LogExtension.LogWarn(context.Exception.Message+","+ context.Exception.StackTrace);

                context.ExceptionHandled = true;
                tip.Status = JsonTip.ERROR;
                tip.Message = context.Exception.Message + "," + context.Exception.StackTrace;
                tip.ReturnUrl =JsonReturnConsts.CLOSE;
                context.Result = Json(tip);
                
                context.Exception = null;

            }

            base.OnActionExecuted(context);
        }



        #region 信息提示页面
        /// <summary>
        /// 显示提示页面
        /// </summary>
        /// <param name="message">提示信息</param>
        /// <param name="type">类型，0失败，1成功</param>
        /// <param name="isCloseDialog">是否关闭掉dialog</param>
        /// <param name="backURL">后退地址，为空则默认上一页</param>
        /// <returns></returns>
        public IActionResult EchoTipPage(string message, int type = 0, bool isCloseDialog = false, string backURL = "")
        {
            ViewBag.Message = message;
            ViewBag.Type = type;
            ViewBag.IsCloseDialog = isCloseDialog;
            ViewBag.BackURL = backURL;
            return View("TipPage");
        }


        /// <summary>
        /// 写入管理员数据库日志
        /// </summary>
        /// <param name="logMsg"></param>
        /// <param name="logType"></param>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        protected async Task WriteLog(string logMsg, AdminLogType logType, AdminLogLevel logLevel)
        {
            try
            {
                Sys_AdminLogModel logModel = new Sys_AdminLogModel();
                logModel.GUID = CookiesHelper.GetCookie(AdminKeyConsts.ADMINLOGID);
                logModel.LogIP = this.HttpContext.GetIPAddress();
                logModel.UId = int.Parse(this.CurrentUserID);
                logModel.UserName = this.CurrentUserName;
                logModel.LogMsg = logMsg;
                logModel.LogLevel = Convert.ToInt16(logLevel);
                logModel.LogType = Convert.ToInt16(logType).ToString();
                logModel.LogTime = DateTime.Now;
                await _adminLogService.InsertAsync(logModel);
            }
            catch (Exception ex)
            {
                LogExtension.LogWarn(string.Format("记录数据库日志出错，原因:{0}", ex.Message));
            }
        }

        /// <summary>
        /// 写入操作日志
        /// </summary>
        /// <param name="logMsg"></param>
        /// <param name="logType"></param>
        /// <param name="logLevel"></param>
        /// <returns></returns>

        protected async Task WriteReviewLog(string code, string logMsg, string controlString, ReviewLogTargetType TargetType, ReviewLogOperateType OperateType)
        {
            try
            {
                Sys_ReviewLogModel reviewlogModel = new Sys_ReviewLogModel();

                reviewlogModel.TargetType = Convert.ToInt32(TargetType);
                reviewlogModel.Code = code;
                reviewlogModel.Operator = int.Parse(this.CurrentUserID);
                reviewlogModel.OperateType = Convert.ToInt32(OperateType);
                if (logMsg.Length > 1000) logMsg = logMsg.Substring(0, 990) + "...";
                reviewlogModel.Remark = logMsg;
                reviewlogModel.OpTime = DateTime.Now;
                reviewlogModel.OperateLocation = controlString;
                await _reviewLogService.InsertAsync(reviewlogModel);
            }
            catch (Exception ex)
            {
                LogExtension.LogWarn(string.Format("记录操作日志出错，原因:{0}", ex.Message));
            }
        }
        #endregion
        /// <summary>
        /// 去除首字母小写
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public JsonResult CustomJson(Object value)
        {
            var serializerSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new DefaultContractResolver()
            };

            //var serializerSettings = new JsonSerializerOptions {
            //    MaxDepth = 4
            //};
            return Json(value, serializerSettings);

        }


    }
}
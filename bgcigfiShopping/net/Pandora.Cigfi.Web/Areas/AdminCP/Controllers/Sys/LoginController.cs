using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pandora.Cigfi.Common;
using Pandora.Cigfi.Models;
using XCode;
using Pandora.Cigfi.Models.Sys;
using FXH.Common.Data.Security;
using Pandora.Cigfi.Services.Sys;
using FXH.Common.Logger;
using NewLife;
using FXH.Web.Extensions.Http;
using Pandora.Cigfi.Models.ResponseMessage;
using Newtonsoft.Json;
using Pandora.Cigfi.Models.Consts;
using System.Text;
using Microsoft.Extensions.Options;
using System.Net.Http;
using Pandora.Cigfi.Models.Sms;
using Microsoft.Extensions.Configuration;
using Pandora.Cigfi.Models.Consts.Sys;
using FXH.Redis.Extensions;
using Aliyun.Acs.Core;
using Aliyun.Acs.afs.Model.V20180112;

namespace Pandora.Cigfi.Web.Areas.AdminCP.Controllers
{
    [Area("AdminCP")]
    public class LoginController : Controller
    {
        private IAdminService _adminService;
        private IAdminLogService _adminLogService;
 
        private CustomHttpClientFactory _httpClientFactory;
        private IRedisCache _redisCache;
        private RequestCheckHelper _requestCheckHelper;
        private IConfigurationManager _configuration;
        private IAcsClient _acsClient;
        private AliyunAFSConfig _afsConfig;
        public LoginController(IConfigurationManager configuration, CustomHttpClientFactory httpClientFactory, IAdminService adminService, IAdminLogService adminLogService,
            IRedisCache redisCache, RequestCheckHelper requestCheckHelper, IAcsClient acsclient, AliyunAFSConfig afsconfig)
        {
            _adminService = adminService;
            _adminLogService = adminLogService;
    
            _httpClientFactory = httpClientFactory;
            _redisCache= redisCache;
            _requestCheckHelper = requestCheckHelper;
            _configuration = configuration;
          _acsClient = acsclient;
          _afsConfig = afsconfig;
        }

        public JsonTip tip = new JsonTip();

        #region 登录页面
        public async Task<IActionResult> Index()
        {
            if (await _adminService.IsLogin())
            {
                return Redirect("/AdminCP");
            }
            string key = CommonUtils.GetRandomChar(12);  
            ViewBag.key = key;
            return View();
        }
        #endregion

        #region 低版本IE界面
        //判断低版本IE
        public IActionResult Ie()
        {
            return View();
        }
        #endregion

        #region 执行登录
        [HttpPost]
       
        public async Task<IActionResult> Login()
        {
            string strUserName = Request.Form["username"];
            string strPassWord = Request.Form["password"];
            string sessionId = Request.Form["sessionId"];
            string sig = Request.Form["sig"];
            string token = Request.Form["token"];
            if (string.IsNullOrEmpty(strUserName)|| string.IsNullOrEmpty(strPassWord))
            {
                tip.Message = "请输入帐号及密码！";
                return Json(tip);
            }
            if (string.IsNullOrEmpty(sessionId) || string.IsNullOrEmpty(sig) || string.IsNullOrEmpty(token))
            {
                tip.Message = "请先通过滑块验证码验证！";
                return Json(tip);
            }

            AuthenticateSigRequest request = new AuthenticateSigRequest();
            request.SessionId = sessionId;// 会话ID。必填参数，从前端获取，不可更改。
            request.Sig = sig;// 签名串。必填参数，从前端获取，不可更改。
            request.Token = token;// 请求唯一标识。必填参数，从前端获取，不可更改。
            request.Scene = _afsConfig.sence;// 场景标识。必填参数，从前端获取，不可更改。
            request.AppKey = _afsConfig.appKey;// 应用类型标识。必填参数，后端填写。
            request.RemoteIp = this.HttpContext.GetIPAddress();// 客户端IP。必填参数，后端填写。
            try {
                //response的code枚举：100验签通过，900验签失败
                AuthenticateSigResponse response = _acsClient.GetAcsResponse(request);
                if (response.Code != 100)
                {
                    tip.Message = "验证不通过！";
                    return Json(tip);
                }
            }
            catch(Exception ex)
            {
                tip.Message = "验证配置存在异常！";
                return Json(tip);
            }
           
            

            string key = Request.Form["des_key"];


            //判断并解密
            //string key = SessionHelper.GetSession("des_key").ToString();
            //解密
            string username = "";
            string password = "";
            try
            {
                username = DESHelper.Decode(strUserName, key);
                password = DESHelper.Decode(strPassWord, key);
            }
            catch (Exception ex)
            {
                LogExtension.LogWarn(string.Format("用户名或者密码错误，原因:{0}", ex.Message));
                tip.Message = "用户名或者密码错误！";
                return Json(tip);
            }

            //验证用户
            if (string.IsNullOrEmpty(username))
            {
                tip.Message = "请输入用户名！" + key;
                return Json(tip);
            }
            if (string.IsNullOrEmpty(password) || CommonUtils.GetStringLength(password) < 5)
            {
                tip.Message = "登录密码不能为空或者长度小于5！";
                return Json(tip);
            }
            //如果15分钟内有10次失败登录，则提示错误
            var ip = this.HttpContext.GetIPAddress();

            if (await _adminLogService.CountFailLoginTimeAsync(ip, DateTime.Now.AddMinutes(-15)) >= 10)
            {
                tip.Message = "错误登录次数限制！";
                return Json(tip);
            }
            //执行登录操作
            if (await _adminService.CheckUserAsync(username, password, ip))
            {
                tip.Status = JsonTip.SUCCESS;
                tip.Message = "登录成功";
                tip.ReturnUrl = "/AdminCP";
                return Json(tip);
            }
            else
            {
                tip.Message = "用户名或者密码错误！请重新登录！";
                return Json(tip);
            }
        }
        #endregion

        /* 手机号登录 【已改用aliyun验证码登录】
        /// <summary>
        /// 验证短信验证码
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CheckSmsCode([FromForm]SmsCodeCheckModel command)
        {
            var ip =  this.HttpContext.GetIPAddress();
            var msg = "登录成功";

            if (command == null || string.IsNullOrEmpty(command.Mobile) || string.IsNullOrEmpty(command.Sms_Code))
            {
                tip.Status = JsonTip.ERROR;
                tip.Message = "输入手机号及验证码";
                return Json(tip);
            }
            //des对称判断并解密
            string key = Request.Form["des_key"];
            try
            {
                command.Mobile = DESHelper.Decode(command.Mobile, key);
                command.Sms_Code = DESHelper.Decode(command.Sms_Code, key);
            }
            catch (Exception ex)
            {
                tip.Status = JsonTip.ERROR;
                tip.Message = "验证失败";
                return Json(tip);
            }
            //判断是否有效手机号
            var isVaildPhone = MobileNoHelper.IsVaildPhoneNumber(command.AreaCode, command.Mobile);
            var  userInfo = await _adminService.GetUserInfoByMobileAsync(command.Mobile);

            if (userInfo == null || !isVaildPhone)
            {
                msg = "异常号码：" + command.Mobile;
                tip.Status = JsonTip.ERROR;
                tip.Message = "用户不存在";
                
                var errorUser = new Sys_AdminModel();
                errorUser.UserName = "异常登录";
                await _adminService.UserloginLogAsync(errorUser, ip, msg, AdminLogLevel.ERROR);
                return Json(tip);
            }
            var fullphone = MobileNoHelper.GetFullMobileNo(command.AreaCode, command.Mobile);
            var ischekrequst = _requestCheckHelper.CheckRequest(RequestCheckType.LoginFailCount, fullphone, ip);
            if (!ischekrequst)
            {
                tip.Status = JsonTip.ERROR;
                tip.Message = "验证登录失败已超过5次，10分钟后重试";
                tip.ReturnUrl = "/AdminCP/Login";
                await _adminService.UserloginLogAsync(userInfo, ip, "登录异常，失败超过5次，手机号: " + command.Mobile, AdminLogLevel.ERROR);
                return Json(tip);
            }

            ResponseMessage<string> message = new ResponseMessage<string>();
#if DEBUG
            bool ischeck = true;
#else
            bool ischeck = _redisCache.Get<string>($"{RedisKeyType.CmsSmsCode}:{fullphone}") == command.Sms_Code;
#endif
            if (ischeck)
            {
                _requestCheckHelper.RemoveCheckRequest(RequestCheckType.LoginFailCount, fullphone, ip);
                tip.Status = JsonTip.SUCCESS;
                tip.Message = msg;
                tip.ReturnUrl = "/AdminCP";
                await _adminService.UserloginLogAsync(userInfo, ip, msg, AdminLogLevel.NORMAL);
            }
            else
            {
                _requestCheckHelper.UpdateCheckRequest(RequestCheckType.LoginFailCount, fullphone, ip);
                tip.Status = JsonTip.ERROR;
                tip.Message = "手机号或验证码错误";
                tip.ReturnUrl = "/AdminCP/Login";
                return Json(tip);
            }
            return Json(tip);
        }

        /// <summary>
        /// 发送短信验证码
        /// </summary>
        [HttpGet]
        public async Task<ResponseMessage<string>> SendSms(string areacode, string mobile)
        {
            string url = _smsConfig.SendUrl;
            var ip = this.HttpContext.GetIPAddress();
            string smscode = new Random().Next(1000, 9999).ToString();
            ResponseMessage<string> message = new ResponseMessage<string>();

            if (!string.IsNullOrEmpty(mobile))
            {
                if (!MobileNoHelper.IsVaildPhoneNumber(areacode, mobile))
                {
                    message.Code = ResponseCode.ERROR;
                    message.Msg = "无效手机号";
                    return message;
                }
                var userInfo = await _adminService.GetUserInfoByMobileAsync(mobile);
                if (userInfo == null)
                {
                    message.Code = ResponseCode.USER_NOTFOUND;
                    message.Msg = "用户不存在";
                    return message;
                }
            }
            else 
            {
                message.Code = ResponseCode.ERROR;
                message.Msg = "输入手机号";
                return message;
            }
            string fullMobileNo = MobileNoHelper.GetFullMobileNo(areacode, mobile);
            var kvs = new List<KeyValuePair<string, string>>();
            kvs.Add(new KeyValuePair<string, string>("phoneno", fullMobileNo));
            string smsContextFormat = _smsConfig.ContextFormat;
            kvs.Add(new KeyValuePair<string, string>("content", smsContextFormat.Replace("{smscode}", smscode)));
            var ischekrequst = _requestCheckHelper.CheckRequest(RequestCheckType.SendSms, fullMobileNo, ip);
            if (!ischekrequst)
            {
                message.Code = ResponseCode.AUTHSUC_NORIGHT;
                message.Msg = "验证码发送过于频繁，10分钟后再试";
                await _adminService.UserloginLogAsync(null, ip, "验证码发送异常，失败超过5次，手机号: " + mobile, AdminLogLevel.ERROR);
                return message;
            }
            var client = _httpClientFactory.CreateClient();
            string result = await CommonUtils.PostFromDataAsync(client, url, kvs);
            var resultdic = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);
            if (resultdic != null && resultdic.ContainsKey("code") && resultdic["code"] == "200")
            {
                message.Code = ResponseCode.OK;
                message.Msg = "验证码已发送";
                _requestCheckHelper.RemoveCheckRequest(RequestCheckType.SendSms, fullMobileNo, ip);
                _redisCache.Add($"{RedisKeyType.CmsSmsCode}:{fullMobileNo}", smscode, TimeSpan.FromMinutes(30));
            }
            else
            {
                _requestCheckHelper.UpdateCheckRequest(RequestCheckType.SendSms, fullMobileNo, ip);
                message.Code = ResponseCode.ERROR;
                message.Msg = result.Contains("发送过于频繁") ==true ? "验证码发送过于频繁，稍后再试" : result;
            }
            return message;
        }
        */


        #region 退出登录
        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Logout()
        {
            _adminService.ClearAdminInfo();
            return Redirect("~/AdminCP/Login");
        }
#endregion
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CustomAuthorizationFilter.Models;
using CustomAuthorizationFilter.Filter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace CustomAuthorizationFilter.Controllers
{
    [TestAuthorizationFilter]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult Login(string user,string pwd)
        {
            if("admin".Equals(user,StringComparison.CurrentCultureIgnoreCase) && pwd.Equals("123456"))
            {
                HttpContext.Response.Cookies.Append("CurrentUser", "admin", new Microsoft.AspNetCore.Http.CookieOptions() {
                Expires = DateTime.UtcNow.AddMinutes(10),
                 HttpOnly = true,
                  MaxAge = TimeSpan.FromMinutes(10)
                });
                return new JsonResult(new { Result = true, Message="登陆成功"});
            }
            else
            {
                return new JsonResult(new { Result = false, Message = "登陆失败" });
            }
        }
        /// <summary>
        /// 可以请求这个地址测试权限
        /// </summary>
        /// <returns></returns>
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

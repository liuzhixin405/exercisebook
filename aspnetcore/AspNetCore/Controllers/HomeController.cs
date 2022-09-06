using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AspNetCore.Models;
using AspNetCore.Extensions;
using AspNetCore.Helper;

namespace AspNetCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDistributedCacheHelper _distributedCacheHelper;
        private readonly IMemoryCacheHelper _memoryCacheHelper;

        public HomeController(ILogger<HomeController> logger, IDistributedCacheHelper distributedCacheHelper, IMemoryCacheHelper memoryCacheHelper)
        {
            _logger = logger;
            _memoryCacheHelper = memoryCacheHelper;
            _distributedCacheHelper = distributedCacheHelper;
        }

        [CustomActionFilter]           //控制器缓存  后台数据 前台数据不会缓存
        public IActionResult Index()
        {
            base.ViewBag.Now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 600, Location = ResponseCacheLocation.Any, NoStore = false)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [ResponseCache(Duration = 60)]    //视图缓存
        public IActionResult InfoCache()
        {
            base.ViewBag.Now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff");
            return View();
        }


        [ResponseCache(Duration = 600)] //谷歌无效，ie生效。 testproject测试 httpclient的Test2方法调用无效
        public string GetValueT()
        {
            return "OK";
        }
    }
}
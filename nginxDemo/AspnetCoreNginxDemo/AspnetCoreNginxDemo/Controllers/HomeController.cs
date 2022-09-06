using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AspnetCoreNginxDemo.Models;
using AspnetCoreNginxDemo.Service;
using AspnetCoreNginxDemo.Injector;

namespace AspnetCoreNginxDemo.Controllers
{
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        private readonly Product product = ServiceLocator.Current.GetInstance<Product>();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index([FromServices] Product pds)
        {
            ViewBag.Host =  HttpContext.Request.Host;
            ViewBag.Port= HttpContext.Request.Host.Port;
            product.Order();

            var pds1 =  (Product)HttpContext.RequestServices.GetService(typeof(Product));
            pds1.Order();
            pds.Order();
            return View();
        }

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

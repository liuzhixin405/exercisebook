using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using  System.Collections.Concurrent;

namespace Pandora.Cigfi.Web.Controllers
{
    public class HomeController : Controller
    {
        //public static IConfiguration Configuration { get; set; }
        //public ConcurrentDictionary<string, string> AllSession = new ConcurrentDictionary<string, string>();
        public HomeController()
        {
//            Configuration = new ConfigurationBuilder()
//.Add(new JsonConfigurationSource { Path = "appsettings.json", ReloadOnChange = true })
//.Build();

        }
        public IActionResult Index()
        {

            Response.Redirect("/admincp");
            

            return View();
        } 
        public IActionResult GetVersion()
        {
            return Content("v1.0.0.2");
        }

    }
}

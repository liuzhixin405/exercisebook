using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System.Reflection;

namespace ControllerWebApplication
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet("Add")]
        public IActionResult Refresh(string dllStr,[FromServices] ApplicationPartManager manager,[FromServices] DynamicChangeTokenProvider tokenProvider)
        {
            try
            {
                manager.ApplicationParts.Add(new AssemblyPart(Assembly.LoadFile($@"{dllStr}")));
                tokenProvider.NotifyChanges();
                return Content("OK");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }//G:\Users\GitHub\exercisebook\动态加载controller\EmbedWebApi\BarLibrary\bin\Debug\net6.0\BarLibrary.dll
    }


}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Embed.Controller.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        public ValuesController()
        {

        }

        [HttpGet]
        public string Get()
        {
            return "OK";
        }
    }
}

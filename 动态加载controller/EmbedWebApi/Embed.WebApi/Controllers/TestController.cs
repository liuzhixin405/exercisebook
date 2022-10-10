using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Embed.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hello World");
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace sample.nettcore6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [ServiceFilter(typeof(ClientIpCheckActionFilter))]
        [Route("get")]
        [HttpGet]
        public Task<string> Get()
        {
            return Task.FromResult("hello world!");
        }
    }
}

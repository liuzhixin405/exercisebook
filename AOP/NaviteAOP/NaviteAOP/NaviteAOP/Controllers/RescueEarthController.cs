using Microsoft.AspNetCore.Mvc;
using NaviteAOP.AOP;
using NaviteAOP.Service.WarServices;

namespace NaviteAOP.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RescueEarthController : ControllerBase
    {
        private IWarService _warService;
        
        public RescueEarthController(IWarService warService)
        {
            _warService = warService;
        }

        [HttpGet(Name = "AnnihilateHegemony")]
        public string AnnihilateHegemony()
        {
            var proxy = _warService.Proxy(_warService); //´úÀí
            return proxy.WipeOut();
        }

        [HttpGet("two")]
        public string AnnihilateHegemonyTwo()
        {
            return _warService.WipeOut();
        }
    }
}
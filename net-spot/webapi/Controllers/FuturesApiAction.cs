using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FuturesApiAction : ControllerBase
    {

        public String MarketKline()
        {
            return "";
        }
    }
}

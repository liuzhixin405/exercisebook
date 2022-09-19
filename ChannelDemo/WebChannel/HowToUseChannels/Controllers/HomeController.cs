using HowToUseChannels.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Channels;

namespace HowToUseChannels.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet("Send")]
        public IActionResult Send()
        {
            return Ok();
        }
        [HttpGet("SendB")]
        public Task<bool> SendB([FromServices] Notifications notifications)
        {
           return notifications.Send();
        }
        [HttpGet("SendA")]
        public bool SendA([FromServices] Notifications notifications)
        {
            return notifications.SendA();
        }
        [HttpGet("SendC")]
        public async Task<bool> SendC([FromServices] Channel<string> channel)
        {
           await channel.Writer.WriteAsync("Hello");
            return true;
        }
    }
}

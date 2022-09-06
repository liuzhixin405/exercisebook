using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orleans.Grains;
using System;
using System.Threading.Tasks;

namespace Orleans.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestOrelansController : ControllerBase
    {
        private readonly IClusterClient _orleansClient;
        public TestOrelansController(IClusterClient orleansClient)
        {
            _orleansClient = orleansClient;
        }

        [HttpGet]
        public async Task<string> GetOrleans1()
        {
            for (int i = 10000; i > 0; i--)
            {
                int j = i;
                await Task.Delay(3000);
                await _orleansClient.GetGrain<IHelloA>(new Random().Next(1, 9)).SayHello("AAAAAAAAAA");
            }
            return "over";
        }
    }
}

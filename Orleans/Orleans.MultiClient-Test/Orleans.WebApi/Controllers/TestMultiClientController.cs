using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orleans.Grains;
using System;
using System.Threading.Tasks;

namespace Orleans.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestMultiClientController : ControllerBase
    {
        private readonly IOrleansClient _orleansClient;
        public TestMultiClientController(IOrleansClient orleansClient)
        {
            _orleansClient = orleansClient;
        }

        [HttpGet]
        public async Task<string> GetOrleans1()
        {
            int sum = 0;
            for (int i = 10; i > 0; i--)
            {
                int temp = i;
                sum += temp;
                await Task.Delay(100);
                var cluster = _orleansClient.GetClusterClient(typeof(IHelloA).Assembly).GetGrain<IHelloA>(0);        
                await cluster.SayHello(DateTime.Now.ToString());
            }
            return sum.ToString();
        }

    }
}

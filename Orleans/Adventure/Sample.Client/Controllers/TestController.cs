using AdventureGrainInterfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;

namespace Sample.Client.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TestController : ControllerBase
    {


        private readonly ILogger<TestController> _logger;
        private readonly IClusterClient _clusterClient;
      
        public TestController(ILogger<TestController> logger, IClusterClient clusterClient)
        {
            _logger = logger;
            _clusterClient = clusterClient;
      
        }

        [HttpGet(Name = "test1")]
        public async Task<string> Get()
        {
            Console.WriteLine(DateTime.Now.ToString());
            return await _clusterClient.GetGrain<IFakeMessage>(Random.Shared.Next()).GetMessage();
        }

    
    }
}

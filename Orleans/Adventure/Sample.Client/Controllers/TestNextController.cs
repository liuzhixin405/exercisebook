using AdventureGrainInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace Sample.Client.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class TestNextController : ControllerBase
    {

        private readonly ILogger<TestNextController> _logger;
        private readonly IClusterClient _clusterClient;

        public TestNextController(ILogger<TestNextController> logger, IClusterClient clusterClient)
        {
            _logger = logger;
            _clusterClient = clusterClient;
        }
        [HttpGet(Name = "test2")]
        public async Task<string> GetNext()
        {
            return await _clusterClient.GetGrain<IFakeMessageNext>(Guid.NewGuid()).GetMessage();
        }
    }
}

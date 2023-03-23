using Comman;
using Dapr.Actors;
using Dapr.Actors.Client;
using Dapr.Client;
using Dapr.Client.Autogen.Grpc.v1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScoreController : ControllerBase
    {
        public ScoreController()
        {
            
        }

        [HttpPut("{scoreId}")]
        public async Task<int> IncrementAsync(string scoreId)
        {
            var daprClient = new DaprClientBuilder().Build();
          
           return await daprClient.InvokeMethodAsync<int>(HttpMethod.Get,"getwf", $"score/increment?scoreId={scoreId}");

        }
    }
}

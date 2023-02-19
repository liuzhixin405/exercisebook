using Dapr;
using Dapr.Client;
using DaprLogger.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Eventing.Reader;

namespace DaprLogger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubController : ControllerBase
    {
        private readonly ILogger _logger;
        public SubController(ILogger<SubController> logger)
        {
            _logger = logger;
        }
        [Topic("pubsub","logging")]
        [HttpPost("Log")]
        public async Task<ActionResult> Log(LogData log)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(log);
            var daprClient = new DaprClientBuilder().Build();
            await daprClient.SaveStateAsync("statestore", "logging", json);
            this._logger.LogInformation(json);
            return Ok(log);
        }
    }
}

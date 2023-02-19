using Dapr.Client;
using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            var daprClient = new DaprClientBuilder().Build();
            var content = daprClient.InvokeMethodAsync<IEnumerable<WeatherForecast>>(HttpMethod.Get, "getwf", "WeatherForecast").Result;
            _logger.LogInformation($"获取wf成功:{content.ToArray().ToString()}");
            return content.ToArray();
        }
    }
}
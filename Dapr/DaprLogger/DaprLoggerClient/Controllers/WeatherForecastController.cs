using DaprLogger.Model;
using Microsoft.AspNetCore.Mvc;

namespace DaprLoggerClient.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<LogData> Get()
        {
            var result = Enumerable.Range(1, 5).Select(index => new LogData
            {
                Id=DateTime.Now.AddDays(index).ToString("yyyy-mm-dd"),
                Level = index,
                Message = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
            _logger.LogInformation(System.Text.Json.JsonSerializer.Serialize(result.FirstOrDefault()));
            return result;
        }
    }
}
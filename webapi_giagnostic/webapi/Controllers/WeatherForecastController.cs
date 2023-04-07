using LogLibrary;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace webapi.Controllers
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
        private readonly DiagnosticSource _diagnosticSource;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, DiagnosticSource diagnosticSource)
        {
            _logger = logger;
            _diagnosticSource = diagnosticSource;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            _logger.BeginScope("xxxxxxxxxx");
            _logger.LogInformation("44444");
            var listener = new DiagnosticListener("MyListener");
            listener.Write("MyEvent", new { Message = "Hello, World!" });
            var listener2 = new DiagnosticListener("webapi");
            listener2.Write("MyEvent", new { Message = "Hello, webapi!" });

            _logger.LogInformation("1111");
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("Rtn")]
        public string Rtn(int id)
        {
            using (_diagnosticSource.StartActivity(new Activity("MyActivity"), "MyActivity"))
            {
                _diagnosticSource.Write("MyCompany.MyEvent", new { Message = "Hello, world!" });
                // ...
            }
            using (_logger.BeginScope(new { Id = id }))
            {
                _logger.LogInformation("Fetching weather forecast...");
                var forecast = new WeatherForecast { Id = id, Date = DateOnly.FromDateTime(DateTime.Now), TemperatureC = 25, Summary = "Sunny" };
                _logger.LogInformation("Weather forecast retrieved: {@forecast}", forecast);
                return "ok";
            }
        }
    }
}
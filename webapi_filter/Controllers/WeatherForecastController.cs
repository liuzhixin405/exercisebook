using Microsoft.AspNetCore.Mvc;

namespace webapi_filter.Controllers;

[ApiController]
[Route("[controller]")]
[TypeFilter(typeof(ExceptionFilter))]
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

    [CustomAuthorize]
    [HttpGet]
    [ResourceFilter]
    [Route("GetWeatherForecast")]
    public IEnumerable<WeatherForecast> GetWeatherForecast()
    {
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

     [HttpGet]
     [Route("ThrowExp")]
    public void ThrowExp()
    {
        throw new Exception("异常测试");
    } 
    [HttpGet]
     [Route("DoNot")]
    public void DoNot()
    {
        System.Console.WriteLine("触发result filter用");
    }

    [HttpGet]
    [Route("More")]
    public void More()
    {
        System.Console.WriteLine("是不是每个方法都会触发middleware执行");
    }
}

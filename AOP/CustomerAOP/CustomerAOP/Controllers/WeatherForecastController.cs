using CustomerAOP.Frameworks;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace CustomerAOP.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly ICusServiceFactory<IWeatherForecastService> _cusServiceFactory;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, ICusServiceFactory<IWeatherForecastService> cusServiceFactory)
        {
            _logger = logger;
            _cusServiceFactory = cusServiceFactory;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            return (await _cusServiceFactory.Invoke("get",null) ) as IEnumerable<WeatherForecast>;
        }

        [HttpGet("greeting")]
        public async Task<string> GreetingAsync(string name)
        {
            var result =await  _cusServiceFactory.Invoke("GreetingAsync", new object[] { name }) as Task<string>;
            return (await result).ToString();
        }
    }
}
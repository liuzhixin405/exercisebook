using AspNetCoreFilters.AttributeModel;
using AspNetCoreFilters.Filters;
using AspNetCoreFilters.Pipeline;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreFilters.Controllers
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

        [HttpGet]
        [AddHeader("Author", "Rick Anderson")]
        [Route("Get")]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet]
        [Route("taf")]
        [ServiceFilter(typeof(MyActionFilterAttribute))]  //稍微复杂 ，绑定配置文件实参映射   只针对单个action，如果全局 放到startup service.Filters.Add(typeof(MyActionFilter))
        public string TestActionFilterAttr()
        {
            return "OK";
        }
        [HttpGet]
        [Route("sr")]
        [ShortCircuitingResourceFilter]
        public IActionResult SomeResource()
        {
            return Content("sucess");
        }
        [ServiceFilter(typeof(AddHeaderResultServiceFilter))]
        [HttpGet]
        [Route("ahrsf")]
        //[TypeFilter(typeof(LogConstantFilter),Arguments = new object[] { "Method 'Hi' called" })]
        public IActionResult AddHeaderResultServiceFilter()
        {
            return Content("OK");
        }
        [HttpGet]
        [Route("AddHeaderFactory")]
        [AddHeaderWithFactory]
        public IActionResult AddHeaderFactory()
        {
            return Content("Examine the headers using the F12 developer tools.");
        }

        [HttpGet]
        [Route("CultureFromRouteData")]
        [MiddlewareFilter(typeof(LocalizationPipeline))]
        public IActionResult CultureFromRouteData()
        {
            return Content(
          $"CurrentCulture:{CultureInfo.CurrentCulture.Name},"
        + $"CurrentUICulture:{CultureInfo.CurrentUICulture.Name}");
        }
    }
}
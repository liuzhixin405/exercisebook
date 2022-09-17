using Microsoft.AspNetCore.Mvc;
using System.Collections.Immutable;
using WebApi.LoggerExtensions;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpDelete(Name = "DeleteById")]
        public IActionResult DeleteById(int id)
        {
            try
            {
                var delEntity = Summaries[id]; // ÑÝÊ¾Çå³ý

                _logger.WfDeleted(delEntity, id);
            }
            catch(NullReferenceException ex)
            {
                _logger.WfDeleteFailed(id, ex);
            }
            return Ok();
        }

        [HttpGet("DeleteAll")]
        public IActionResult RemoveAll()
        {
            var count = Summaries.Count();
            using (_logger._wfsDeletedScope(count))
            {
                for (int i = 0; i < Summaries.Length; i++)
                {
                    //É¾³ý
                    _logger.WfDeleted(Summaries[i], i);
                }
            }
            return Ok();
        }
    }

  
}
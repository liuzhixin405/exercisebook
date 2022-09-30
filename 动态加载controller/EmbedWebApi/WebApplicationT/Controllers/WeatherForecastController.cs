using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace WebApplicationT.Controllers
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

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IServiceProvider serviceDescriptors)
        {
            _logger = logger;
            if (serviceDescriptors == null)
                throw new ArgumentNullException($"{nameof(serviceDescriptors)} is null");
            var provider = serviceDescriptors.GetType().GetProperty("RootProvider", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
          
            var serviceField = provider.PropertyType.GetField("_realizedServices", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            //Type t = serviceField.FieldType;
            //object o = Activator.CreateInstance(t,);
            var serviceval = serviceField.GetValue(provider.DeclaringType);
            var funcType = serviceField.FieldType.GetGenericArguments()[1].GetGenericArguments()[0];
            var method = serviceField.FieldType.GetMethods().Where(s => s.Name == "GetOrAdd").ToArray()[2];
            var result = typeof(WeatherForecastController).GetMethods().FirstOrDefault(s => s.Name == "GetFunc").MakeGenericMethod(funcType);
            method.Invoke(serviceval, new object[] { typeof(OrderService), result });//ÄÇ²»ºÃservicecollection
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

        [HttpGet("Txqz")]
        public string Txqz([FromServices] IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateAsyncScope();
            var service = scope.ServiceProvider.GetRequiredService<OrderService>();
            return service.Test();
        }


        //[HttpGet("Asdf")]
        //public string Asdf([FromServices] IServiceCollection services)
        //{
        //    services.TryAddTransient<OrderService>();
        //    var serviceProvider = services.BuildServiceProvider();
        //    using var scope = serviceProvider.CreateAsyncScope();
        //    var service = scope.ServiceProvider.GetRequiredService<OrderService>();
        //    return service.Test();
        //}
    }
}
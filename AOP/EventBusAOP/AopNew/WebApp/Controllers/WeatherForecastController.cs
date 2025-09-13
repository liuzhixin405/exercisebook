using Common.Bus.Core;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
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
        private readonly ICommandBus _commandBus;
        public WeatherForecastController(ILogger<WeatherForecastController> logger,ICommandBus commandBus)
        {
            _logger = logger;
            _commandBus = commandBus;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<IEnumerable<WeatherForecast>> Get(int num)
        {
            var result = await _commandBus.SendAsync<CreateOrderCommand, string>(new CreateOrderCommand("iPhone", num));
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

    }

    public record CreateOrderCommand(string Product, int Quantity) : ICommand<string>;
    public class CreateOrderHandler : ICommandHandler<CreateOrderCommand, string>
    {
        public Task<string> HandleAsync(CreateOrderCommand command, CancellationToken ct = default)
            => Task.FromResult($"Order Created: {command.Product} x {command.Quantity}");
    }
}

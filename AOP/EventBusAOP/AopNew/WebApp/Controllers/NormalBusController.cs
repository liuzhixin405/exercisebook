using Common.Bus.Core;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NormalBusController : ControllerBase
    {
      

        private readonly ILogger<NormalBusController> _logger;
        private readonly ICommandBus _commandBus;
        public NormalBusController(ILogger<NormalBusController> logger,ICommandBus commandBus)
        {
            _logger = logger;
            _commandBus = commandBus;
        }

        [HttpGet(Name = "createorder")]
        public async Task<string> CreateOrder(int num)
        {
            var result = await _commandBus.SendAsync<CreateOrderCommand, string>(new CreateOrderCommand("iPhone", num));

            return result;
        }

    }

    public record CreateOrderCommand(string Product, int Quantity) : ICommand<string>;
    public class CreateOrderHandler : ICommandHandler<CreateOrderCommand, string>
    {
        public Task<string> HandleAsync(CreateOrderCommand command, CancellationToken ct = default)
            => Task.FromResult($"Order Created: {command.Product} x {command.Quantity}");
    }
}

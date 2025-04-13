using Microsoft.AspNetCore.Mvc;
using spot.Application.Features.Orders.Commands.CancelOrder;
using spot.Application.Features.Orders.Commands.CreateOrder;
using spot.Application.Features.Orders.Queries.GetOrdersByUserId;
using System.Threading.Tasks;

namespace spot.WebApi.Controllers.v1
{
    public class OrderController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetByUserId([FromQuery] GetOrdersByUserIdQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Cancel(string id, [FromQuery] string userId)
        {
            var command = new CancelOrderCommand { OrderId = id, UserId = userId };
            return Ok(await Mediator.Send(command));
        }
    }
}
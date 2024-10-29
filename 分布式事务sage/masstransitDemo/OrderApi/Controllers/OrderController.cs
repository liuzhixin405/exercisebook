using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Repository.Service.Dtos.Orders;
using Repository.Service.Orders;

namespace OrderApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;
        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]

        public async Task<IActionResult> CreateOrder([FromBody]OrderDto orderDto)
        {
            var result =await _orderService.CreateOrderAsync(new Order { CustomerName = orderDto.CustomerName, TotalAmount = orderDto.TotalAmount, OrderDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds() });

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<int> Delete(int id)
        {
            return await _orderService.DeleteOrderAsync(id);
        }
    }


}

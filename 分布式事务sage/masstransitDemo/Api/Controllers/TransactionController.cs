using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.Service.Consumers;
using Repository.Service.Dtos;
namespace OrderApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {

        private readonly IBus _bus;

        public TransactionController(IBus bus)
        {
            _bus = bus;
        }

        [HttpPost("CreateTransaction")]
        public async Task<IActionResult> CreateTransaction([FromBody]CreateDto dto)
        {
            await _bus.Publish<ICreateTransaction>(dto);
            return Ok("Transaction processing started.");
        }

        // Client/CreateTransactionDto.cs
        public class CreateTransactionDto
        {
            public decimal Amount { get; set; }
            public string Description { get; set; }
        }
    }
}

using Contract.Core.Entities;
using Contract.Core.Enum;
using Contract.Core.Interfaces;
using Contract.SharedKernel.Interfaces;
using Contract.Web.ApiModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Contract.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IRepository<Order> repository;
        public OrderController(IRepository<Order> repository)
        {
            this.repository = repository;
        }
        [HttpPost]
        public async Task<IActionResult> OpenOrderAsync([FromBody] OrderDTO orderDTO)
        {
            var order = new Order()
            {
                 ContractId = orderDTO.ContractId,
                 CumQty = orderDTO.CumQty,
                 MarginMode= orderDTO.MarginMode,
                 Price = orderDTO.Price
            };
            var result = await repository.AddAsync(order);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Complete(string contractId,string itemId)
        {
            return Ok();
        }
    }
}

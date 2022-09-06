using CqrsWithEs.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CqrsWithEs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PoliciesController : ControllerBase
    {
        private readonly IMediator mediator;

        public PoliciesController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePolicy([FromBody] CreatePolicyCommand cmd)
        {
            var result = await mediator.Send(cmd);
            return Ok(result);
        }

        [HttpPost("BuyAdditionalCover")]
        public async Task<IActionResult> BuyAdditionalCover([FromBody] BuyAdditionalCoverCommand cmd)
        {
            var result = await mediator.Send(cmd);
            return Ok(result);
        }

        [HttpPost("ConfirmBuyAdditionalCover")]
        public async Task<IActionResult> Post([FromBody] ConfirmBuyAdditionalCoverCommand cmd)
        {
            var result = await mediator.Send(cmd);
            return Ok(result);
        }

        [HttpPost("Terminate")]
        public async Task<IActionResult> Terminate([FromBody] TerminatePolicyCommand cmd)
        {
            var result = await mediator.Send(cmd);
            return Ok(result);
        }

        [HttpPost("ConfirmTermination")]
        public async Task<IActionResult> ConfirmTermination([FromBody] ConfirmTerminationCommand cmd)
        {
            var result = await mediator.Send(cmd);
            return Ok(result);
        }
        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        [HttpGet("OfferAll")]
        public async Task<IActionResult> OfferAll()
        {
            var res = await mediator.Send(new OfferListCommand());
            return Ok(res);
        }
        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("OfferById/{id}")]
        public async Task<IActionResult> OfferById(string id)
        {
            if(Guid.TryParse(id, out var result))
            {
                var res = await mediator.Send(new OfferByIdCommand(result));
                return Ok(res);
            }
           return NotFound();  
        }
    }
}

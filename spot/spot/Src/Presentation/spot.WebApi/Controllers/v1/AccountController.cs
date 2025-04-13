using Microsoft.AspNetCore.Mvc;
using spot.Application.Features.Accounts.Commands.CreateAccount;
using spot.Application.Features.Accounts.Queries.GetAccountsByUserId;
using System.Threading.Tasks;

namespace spot.WebApi.Controllers.v1
{
    public class AccountController : BaseApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string userId)
        {
            return Ok(await Mediator.Send(new GetAccountsByUserIdQuery { UserId = userId }));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAccountCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
    }
}
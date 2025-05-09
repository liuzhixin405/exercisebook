using Microsoft.AspNetCore.Mvc;
using spot.Application.DTOs.Account.Requests;
using spot.Application.Interfaces.UserInterfaces;
using System.Threading.Tasks;

namespace spot.WebApi.Controllers.v1
{
    [ApiVersion("1")]
    public class AuthController : BaseApiController
    {
        private readonly IAccountServices _accountService;

        public AuthController(IAccountServices accountService)
        {
            _accountService = accountService;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] AuthenticationRequest request)
        {
            var result = await _accountService.Authenticate(request);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> LoginByUserName([FromQuery] string userName)
        {
            var result = await _accountService.AuthenticateByUserName(userName);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var result = await _accountService.ChangePassword(request);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUserName([FromBody] ChangeUserNameRequest request)
        {
            var result = await _accountService.ChangeUserName(request);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> RegisterGhostAccount()
        {
            var result = await _accountService.RegisterGhostAccount();
            return Ok(result);
        }
    }
} 
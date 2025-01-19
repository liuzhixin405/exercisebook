using Microsoft.AspNetCore.Mvc;
using webapi.Services;
using webapi.Services.Impl;

namespace webapi.Controllers
{
    [ApiController]
    [Route("controller")]
    public class AccountController : ControllerBase
    {
        private readonly IBqAccountService _bqAccountService;

        private readonly ILogger<AccountController> _logger;

        public AccountController(ILogger<AccountController> logger, IBqAccountService bQAccountService)
        {
            _logger = logger;
            _bqAccountService = bQAccountService;
        }

        [HttpGet("getbyuserandcurrency")]
        public async Task<BqAccount> GetByUserAndCurrency(long uid,long cid)
        {
            return await _bqAccountService.GetByUserAndCurrency(uid, cid);
        }
    }
}

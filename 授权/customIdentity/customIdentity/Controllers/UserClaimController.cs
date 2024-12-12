using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace customIdentity.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserClaimController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserClaimController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("get-claims/{email}")]
        public async Task<IActionResult> GetClaimsByUser(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var claims = await _userManager.GetClaimsAsync(user);
            return Ok(claims);
        }
    }

}

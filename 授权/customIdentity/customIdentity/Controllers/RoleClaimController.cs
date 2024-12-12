using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace customIdentity.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleClaimController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleClaimController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [HttpPost("add-claim")]
        public async Task<IActionResult> AddClaimToRole([FromBody] AddClaimDto model)
        {
            var role = await _roleManager.FindByNameAsync(model.RoleName);
            if (role == null)
            {
                return NotFound("Role not found.");
            }

            var claim = new Claim(model.ClaimType, model.ClaimValue);
            var result = await _roleManager.AddClaimAsync(role, claim);

            if (result.Succeeded)
            {
                return Ok("Claim added to role successfully.");
            }

            return BadRequest(result.Errors);
        }

        [HttpGet("get-claims/{roleName}")]
        public async Task<IActionResult> GetClaimsByRole(string roleName)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                return NotFound("Role not found.");
            }

            var claims = await _roleManager.GetClaimsAsync(role);
            return Ok(claims);
        }
      
        [HttpDelete("remove-claim")]
        public async Task<IActionResult> RemoveClaimFromRole([FromBody] RemoveClaimDto model)
        {
            var role = await _roleManager.FindByNameAsync(model.RoleName);
            if (role == null)
            {
                return NotFound("Role not found.");
            }

            var claims = await _roleManager.GetClaimsAsync(role);
            var claim = claims.FirstOrDefault(c => c.Type == model.ClaimType && c.Value == model.ClaimValue);
            if (claim == null)
            {
                return NotFound("Claim not found.");
            }

            var result = await _roleManager.RemoveClaimAsync(role, claim);
            if (result.Succeeded)
            {
                return Ok("Claim removed from role.");
            }

            return BadRequest(result.Errors);
        }

     
    }

}

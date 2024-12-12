using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace customIdentity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public PermissionsController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // 创建新的权限（Claim）并分配给用户
        [HttpPost("add-claim")]
        [Authorize(Roles = "Admin")] // 只有 Admin 可以添加权限
        public async Task<IActionResult> AddClaimToUser([FromBody] AddClaimRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.UserEmail);
            if (user == null)
            {
                return NotFound("User not found");
            }

            // 检查用户是否已有此权限（防止重复添加）
            var existingClaim = await _userManager.GetClaimsAsync(user);
            if (existingClaim.Any(c => c.Type == "Permission" && c.Value == request.Permission))
            {
                return BadRequest("Claim already exists");
            }

            // 给用户添加新的 Claim
            var claim = new Claim("Permission", request.Permission);
            var result = await _userManager.AddClaimAsync(user, claim);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok($"Claim '{request.Permission}' added to user {request.UserEmail}");
        }
    }

    public class AddClaimRequest
    {
        public string UserEmail { get; set; }
        public string Permission { get; set; }
    }
}

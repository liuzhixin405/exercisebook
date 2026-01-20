using System.Linq;
using IdentityPaymentApi.DTOs;
using IdentityPaymentApi.Models;
using IdentityPaymentApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace IdentityPaymentApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ITokenService _tokenService;
    private readonly JwtSettings _jwtSettings;

    public AuthController(UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ITokenService tokenService,
        IOptions<JwtSettings> jwtSettings)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
        _jwtSettings = jwtSettings.Value;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                Errors = result.Errors.Select(error => error.Description)
            });
        }

        if (!await _roleManager.RoleExistsAsync("Payer"))
        {
            await _roleManager.CreateAsync(new IdentityRole("Payer"));
        }

        await _userManager.AddToRoleAsync(user, "Payer");
        return CreatedAtAction(nameof(Register), new { user.Id, user.Email }, new { user.Id, user.Email });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return Unauthorized(new { Message = "Invalid credentials." });
        }

        var token = await _tokenService.CreateTokenAsync(user);
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);
        return Ok(new TokenResponse { Token = token, ExpiresAt = expiresAt });
    }
}
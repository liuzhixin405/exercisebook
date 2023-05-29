using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace auth_cookie.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        //[HttpPost("login")]
        [HttpGet("login")] //方便测试
        public async Task<IActionResult> Login(string username, string password)
        {
            // 执行验证用户名和密码的逻辑
            //这里可以和存到数据库的用户和密码进行比对
            if(username != "admin" || password != "123456")
            {
                return BadRequest("Invalid username or password");
            }
            // 如果验证成功，创建身份验证Cookie
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "Admin") // 添加用户角色
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties());

            return Ok("Login successful");
        }

        //[HttpPost("logout")]
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok("Logout successful");
        }
    }
}

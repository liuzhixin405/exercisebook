using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.API.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        /// <summary>
        /// 当前用户ID（未登录时为 null）
        /// </summary>
        protected Guid? CurrentUserId
        {
            get
            {
                var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return Guid.TryParse(userId, out var guid) ? guid : null;
            }
        }

        /// <summary>
        /// 当前用户名
        /// </summary>
        protected string? CurrentUserName => User?.Identity?.Name;

        /// <summary>
        /// 当前用户是否为管理员
        /// </summary>
        protected bool IsAdmin => User?.IsInRole("Admin") ?? false;
    }
}
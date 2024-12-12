using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace customIdentity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        // 模拟一些数据
        private readonly List<string> orders = new List<string>
        {
            "Order 1 - $100",
            "Order 2 - $200",
            "Order 3 - $300"
        };

        // 通过角色访问：只有 Admin 角色的用户可以访问
        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAdminOrders()
        {
            return Ok(orders);
        }

        // 通过权限 Claim 访问：只有具有 'Permission' Claim 为 'ViewOrders' 的用户可以访问
        [HttpGet("permission")]
        [Authorize(Policy = "ViewOrdersPolicy")]
        public IActionResult GetOrdersByPermission()
        {
            return Ok(orders);
        }

        // 公共接口，所有用户都可以访问
        [HttpGet("public")]
        public IActionResult GetPublicOrders()
        {
            return Ok(orders);
        }
    }
}

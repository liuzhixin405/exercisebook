using Cat.Seckill.Base.EFCore.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cat.Seckill.HttpApi.Order.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoodsController : ControllerBase
    {
        private
            readonly IOrderService orderService;
        public GoodsController(IOrderService orderService)
        {
            this.orderService= orderService;
        }
    }
}

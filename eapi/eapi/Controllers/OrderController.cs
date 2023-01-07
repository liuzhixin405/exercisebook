using eapi.Models;
using eapi.Models.Dtos;
using eapi.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Channels;

namespace eapi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService orderService;
        public OrderController(IOrderService orderService)
        {
            this.orderService = orderService;//500请求 并发50 . 100库存
        }

        [HttpPost]
        public async Task Create([FromServices]Channel<CreateOrderDto> channel,string sku, int count)
        {
           await channel.Writer.WriteAsync(new CreateOrderDto(sku,count));   //高并发高效解决方案  并发测试工具postjson_windows
        }

        [HttpPost]
        public async Task CreateTestLock(string sku, int count)
        {
            await orderService.CreateTestLock(sku, count); //执行时间快,库存少量扣减
        }

        [HttpPost]
        public async Task CreateDistLock(string sku, int count)
        {
            await orderService.CreateDistLock(sku, count); //库存扣完，时间长
        }
        [HttpGet]
        public async Task ChangeOrderStatus(int orderId, OrderStatus status)
        {
            switch (status)
            {
                case OrderStatus.Shipment:
                    await orderService.Shipment(orderId);
                    break;
                case OrderStatus.Completed:
                    await orderService.Completed(orderId);
                    break;
                case OrderStatus.Rejected:
                    await orderService.Rejected(orderId);
                    break;
                default:
                    break;
            }
        }
    }
}

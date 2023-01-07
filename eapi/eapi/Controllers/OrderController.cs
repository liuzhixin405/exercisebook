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
            this.orderService = orderService;
        }

        [HttpPost]
        public async Task Create([FromServices]Channel<CreateOrderDto> channel,string sku, int count)
        {
           await channel.Writer.WriteAsync(new CreateOrderDto(sku,count));   //高并发高效解决方案  并发测试工具postjson_windows
        }

        [HttpPost]
        public async Task CreateTestLock(string sku, int count)
        {
            await orderService.CreateTestLock(sku, count); //效率不高
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

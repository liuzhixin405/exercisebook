using eapi.Models;
using eapi.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public async Task Create(string sku, int count)
        {
            await orderService.Create(sku, count);
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

using ECommerce.API.Models;

namespace ECommerce.API.Application
{
    // 基础订单服务接口
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(OrderRequest request);
        Task<Order> GetOrderAsync(int orderId);
    }
}

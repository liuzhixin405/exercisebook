using ECommerce.API.Models;

namespace ECommerce.API.Application
{
    // 支付策略接口
    public interface IPaymentStrategy
    {
        Task<PaymentResult> ProcessPaymentAsync(decimal amount);
    }
}

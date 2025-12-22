using ECommerce.API.Application;
using ECommerce.API.Models;

namespace ECommerce.API.Infrastructure
{
    // PayPal支付
    public class PayPalPayment : IPaymentStrategy
    {
        private readonly ILogger<PayPalPayment> _logger;

        public PayPalPayment(ILogger<PayPalPayment> logger)
        {
            _logger = logger;
        }

        public async Task<PaymentResult> ProcessPaymentAsync(decimal amount)
        {
            _logger.LogInformation($"Processing PayPal payment: {amount}");
            // PayPal处理逻辑
            await Task.Delay(150);
            return new PaymentResult { Success = true, TransactionId = Guid.NewGuid() };
        }
    }
}

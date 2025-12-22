using ECommerce.API.Application;
using ECommerce.API.Models;

namespace ECommerce.API.Infrastructure
{
    // 信用卡支付
    public class CreditCardPayment : IPaymentStrategy
    {
        private readonly ILogger<CreditCardPayment> _logger;

        public CreditCardPayment(ILogger<CreditCardPayment> logger)
        {
            _logger = logger;
        }

        public async Task<PaymentResult> ProcessPaymentAsync(decimal amount)
        {
            _logger.LogInformation($"Processing credit card payment: {amount}");
            // 信用卡处理逻辑
            await Task.Delay(100);
            return new PaymentResult { Success = true, TransactionId = Guid.NewGuid() };
        }
    }
}

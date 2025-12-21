using ECommerce.API.Application;

namespace ECommerce.API.Infrastucture
{
    public class PaymentStrategyFactory : IPaymentStrategyFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public PaymentStrategyFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IPaymentStrategy CreateStrategy(string paymentType)
        {
            return paymentType.ToLower() switch
            {
                "creditcard" => _serviceProvider.GetRequiredService<CreditCardPayment>(),
                "paypal" => _serviceProvider.GetRequiredService<PayPalPayment>(),
                _ => throw new ArgumentException($"Unsupported payment type: {paymentType}")
            };
        }
    }
}

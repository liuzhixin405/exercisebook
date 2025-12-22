namespace ECommerce.API.Application
{
    // 支付策略工厂
    public interface IPaymentStrategyFactory
    {
        IPaymentStrategy CreateStrategy(string paymentType);
    }
}

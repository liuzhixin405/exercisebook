using Microsoft.Extensions.Logging;

namespace designdemo
{
    internal class PaymentProcessor<T> where T : IPaymentEntity
    {
        private readonly IPayment<T> _paymentAdapter;
        private readonly ILogger<PaymentProcessor<T>> _logger;

        public PaymentProcessor(IPayment<T> paymentAdapter, ILogger<PaymentProcessor<T>> logger)
        {
            _paymentAdapter = paymentAdapter;
            _logger = logger;
        }
        public void ProcessPayment(T payment)
        {
            try
            {
                _logger.LogInformation("开始处理支付请求，交易ID：{TransactionId}", payment.TransactionId);
                _paymentAdapter.Pay(payment);
                _logger.LogInformation("支付处理成功，交易ID：{TransactionId}", payment.TransactionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "支付处理失败，交易ID：{TransactionId} 错误信息：{ErrorMessage}",
                    payment.TransactionId, ex.Message);
                throw new PaymentException("支付处理过程中发生错误", ex);
            }
        }
    }
}

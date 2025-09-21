using ECommerce.Domain.Models;

namespace ECommerce.Domain.Interfaces
{
    public interface IPaymentService
    {
        /// <summary>
        /// 处理支付
        /// </summary>
        /// <param name="paymentRequest">支付请求</param>
        /// <returns>支付结果</returns>
        Task<PaymentResult> ProcessPaymentAsync(PaymentRequest paymentRequest);
        
        /// <summary>
        /// 验证支付
        /// </summary>
        /// <param name="paymentId">支付ID</param>
        /// <returns>验证结果</returns>
        Task<PaymentValidationResult> ValidatePaymentAsync(string paymentId);
        
        /// <summary>
        /// 退款处理
        /// </summary>
        /// <param name="refundRequest">退款请求</param>
        /// <returns>退款结果</returns>
        Task<RefundResult> ProcessRefundAsync(RefundRequest refundRequest);
        
        /// <summary>
        /// 获取支付状态
        /// </summary>
        /// <param name="orderId">订单ID</param>
        /// <returns>支付状态</returns>
        Task<PaymentStatus> GetPaymentStatusAsync(Guid orderId);
    }
}

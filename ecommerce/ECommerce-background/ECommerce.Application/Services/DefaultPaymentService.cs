using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Models;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Services
{
    /// <summary>
    /// 默认支付服务实现
    /// 这是一个模拟实现，在实际生产环境中应该集成真实的支付网关
    /// </summary>
    public class DefaultPaymentService : IPaymentService
    {
        private readonly ILogger<DefaultPaymentService> _logger;
        private readonly Dictionary<Guid, PaymentStatus> _paymentRecords = new();

        public DefaultPaymentService(ILogger<DefaultPaymentService> logger)
        {
            _logger = logger;
        }

        public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest paymentRequest)
        {
            _logger.LogInformation("Processing payment for order: {OrderId}, Amount: {Amount}, Method: {Method}", 
                paymentRequest.OrderId, paymentRequest.Amount, paymentRequest.PaymentMethod);

            try
            {
                // 模拟支付处理延迟
                await Task.Delay(100);

                // 生成支付ID和交易ID
                var paymentId = Guid.NewGuid().ToString();
                var transactionId = Guid.NewGuid().ToString();

                // 创建支付状态记录
                var paymentStatus = new PaymentStatus
                {
                    PaymentId = paymentId,
                    OrderId = paymentRequest.OrderId,
                    State = PaymentState.Completed,
                    Amount = paymentRequest.Amount,
                    Currency = paymentRequest.Currency,
                    PaymentMethod = paymentRequest.PaymentMethod,
                    CreatedAt = DateTime.UtcNow,
                    ProcessedAt = DateTime.UtcNow,
                    Message = "Payment processed successfully"
                };

                // 存储支付记录
                _paymentRecords[paymentRequest.OrderId] = paymentStatus;

                var result = new PaymentResult
                {
                    Success = true,
                    PaymentId = paymentId,
                    TransactionId = transactionId,
                    Status = paymentStatus,
                    Message = "Payment completed successfully",
                    ProcessedAt = DateTime.UtcNow,
                    AdditionalData = new Dictionary<string, string>
                    {
                        { "Gateway", "DefaultPaymentGateway" },
                        { "ProcessingTime", "100ms" },
                        { "Simulated", "true" }
                    }
                };

                _logger.LogInformation("Payment completed successfully for order: {OrderId}, PaymentId: {PaymentId}", 
                    paymentRequest.OrderId, paymentId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Payment processing failed for order: {OrderId}", paymentRequest.OrderId);

                var failedResult = new PaymentResult
                {
                    Success = false,
                    PaymentId = string.Empty,
                    TransactionId = string.Empty,
                    Status = new PaymentStatus
                    {
                        PaymentId = string.Empty,
                        OrderId = paymentRequest.OrderId,
                        State = PaymentState.Failed,
                        Amount = paymentRequest.Amount,
                        Currency = paymentRequest.Currency,
                        PaymentMethod = paymentRequest.PaymentMethod,
                        CreatedAt = DateTime.UtcNow,
                        Message = $"Payment failed: {ex.Message}"
                    },
                    Message = $"Payment processing failed: {ex.Message}",
                    ProcessedAt = DateTime.UtcNow
                };

                return failedResult;
            }
        }

        public async Task<PaymentValidationResult> ValidatePaymentAsync(string paymentId)
        {
            _logger.LogInformation("Validating payment: {PaymentId}", paymentId);

            try
            {
                // 模拟验证延迟
                await Task.Delay(50);

                // 查找支付记录
                var payment = _paymentRecords.Values.FirstOrDefault(p => p.PaymentId == paymentId);
                
                if (payment == null)
                {
                    return new PaymentValidationResult
                    {
                        IsValid = false,
                        PaymentId = paymentId,
                        Status = new PaymentStatus
                        {
                            State = PaymentState.Failed,
                            Message = "Payment not found"
                        },
                        Message = "Payment not found",
                        ValidatedAt = DateTime.UtcNow
                    };
                }

                var result = new PaymentValidationResult
                {
                    IsValid = payment.State == PaymentState.Completed,
                    PaymentId = paymentId,
                    Status = payment,
                    Message = payment.State == PaymentState.Completed ? "Payment is valid" : "Payment is not valid",
                    ValidatedAt = DateTime.UtcNow
                };

                _logger.LogInformation("Payment validation completed for: {PaymentId}, IsValid: {IsValid}", 
                    paymentId, result.IsValid);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Payment validation failed for: {PaymentId}", paymentId);

                return new PaymentValidationResult
                {
                    IsValid = false,
                    PaymentId = paymentId,
                    Status = new PaymentStatus
                    {
                        State = PaymentState.Failed,
                        Message = $"Validation failed: {ex.Message}"
                    },
                    Message = $"Payment validation failed: {ex.Message}",
                    ValidatedAt = DateTime.UtcNow
                };
            }
        }

        public async Task<RefundResult> ProcessRefundAsync(RefundRequest refundRequest)
        {
            _logger.LogInformation("Processing refund for order: {OrderId}, Amount: {Amount}", 
                refundRequest.OrderId, refundRequest.Amount);

            try
            {
                // 模拟退款处理延迟
                await Task.Delay(150);

                // 生成退款ID
                var refundId = Guid.NewGuid().ToString();

                var result = new RefundResult
                {
                    Success = true,
                    RefundId = refundId,
                    PaymentId = refundRequest.PaymentId,
                    Amount = refundRequest.Amount,
                    Status = RefundStatus.Completed,
                    Message = "Refund processed successfully",
                    ProcessedAt = DateTime.UtcNow
                };

                _logger.LogInformation("Refund completed successfully for order: {OrderId}, RefundId: {RefundId}", 
                    refundRequest.OrderId, refundId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Refund processing failed for order: {OrderId}", refundRequest.OrderId);

                return new RefundResult
                {
                    Success = false,
                    RefundId = string.Empty,
                    PaymentId = refundRequest.PaymentId,
                    Amount = refundRequest.Amount,
                    Status = RefundStatus.Failed,
                    Message = $"Refund processing failed: {ex.Message}",
                    ProcessedAt = DateTime.UtcNow
                };
            }
        }

        public async Task<PaymentStatus> GetPaymentStatusAsync(Guid orderId)
        {
            _logger.LogInformation("Getting payment status for order: {OrderId}", orderId);

            try
            {
                // 模拟查询延迟
                await Task.Delay(30);

                if (_paymentRecords.TryGetValue(orderId, out var paymentStatus))
                {
                    return paymentStatus;
                }

                // 如果没有找到支付记录，返回默认状态
                return new PaymentStatus
                {
                    PaymentId = string.Empty,
                    OrderId = orderId,
                    State = PaymentState.Pending,
                    Amount = 0,
                    Currency = "CNY",
                    PaymentMethod = string.Empty,
                    CreatedAt = DateTime.UtcNow,
                    Message = "No payment record found"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get payment status for order: {OrderId}", orderId);

                return new PaymentStatus
                {
                    PaymentId = string.Empty,
                    OrderId = orderId,
                    State = PaymentState.Failed,
                    Amount = 0,
                    Currency = "CNY",
                    PaymentMethod = string.Empty,
                    CreatedAt = DateTime.UtcNow,
                    Message = $"Failed to get payment status: {ex.Message}"
                };
            }
        }
    }
}

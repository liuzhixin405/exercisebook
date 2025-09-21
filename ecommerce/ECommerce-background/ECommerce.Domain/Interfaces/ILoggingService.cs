using ECommerce.Domain.Events;

namespace ECommerce.Domain.Interfaces
{
    /// <summary>
    /// 日志服务接口
    /// </summary>
    public interface ILoggingService
    {
        /// <summary>
        /// 记录事件日志
        /// </summary>
        Task LogEventAsync<T>(T @event, string status = "pending", string? errorMessage = null) where T : class;

        /// <summary>
        /// 记录事件处理结果
        /// </summary>
        Task LogEventProcessingAsync(string eventId, string handlerType, bool success, int processingTimeMs, string? errorMessage = null);

        /// <summary>
        /// 记录业务操作日志
        /// </summary>
        Task LogBusinessOperationAsync(string operationType, string entityType, string entityId, object? operationData = null, string? operatorName = null, string? correlationId = null, bool success = true, string? errorMessage = null);

        /// <summary>
        /// 记录库存变更日志
        /// </summary>
        Task LogInventoryChangeAsync(Guid productId, int oldStock, int newStock, string operationType, string reason, Guid? relatedOrderId = null, string? operatorName = null, string? correlationId = null);

        /// <summary>
        /// 记录订单状态变更日志
        /// </summary>
        Task LogOrderStatusChangeAsync(Guid orderId, string oldStatus, string newStatus, string? reason = null, string? operatorName = null, object? statusData = null, string? correlationId = null);

        /// <summary>
        /// 记录支付处理日志
        /// </summary>
        Task LogPaymentProcessingAsync(string paymentId, Guid orderId, decimal amount, string paymentMethod, string status, string? transactionId = null, string? errorMessage = null, int? processingTimeMs = null, string? correlationId = null);

        /// <summary>
        /// 获取事件处理统计
        /// </summary>
        Task<object> GetEventProcessingStatsAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// 获取业务操作统计
        /// </summary>
        Task<object> GetBusinessOperationStatsAsync(DateTime startDate, DateTime endDate);
    }
}

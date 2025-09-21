using ECommerce.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data;
using MySql.Data.MySqlClient;

namespace ECommerce.Infrastructure.Services
{
    /// <summary>
    /// 日志服务实现
    /// </summary>
    public class LoggingService : ILoggingService
    {
        private readonly ILogger<LoggingService> _logger;
        private readonly string _connectionString;

        public LoggingService(ILogger<LoggingService> logger, string connectionString)
        {
            _logger = logger;
            _connectionString = connectionString;
        }

        /// <summary>
        /// 记录事件日志
        /// </summary>
        public async Task LogEventAsync<T>(T @event, string status = "pending", string? errorMessage = null) where T : class
        {
            try
            {
                var eventType = @event.GetType().Name;
                var eventId = GetEventId(@event);
                var correlationId = GetCorrelationId(@event);
                var source = GetEventSource(@event);
                var eventData = JsonConvert.SerializeObject(@event);

                var sql = @"
                    INSERT INTO event_logs (event_id, event_type, correlation_id, source, event_data, status, error_message)
                    VALUES (@eventId, @eventType, @correlationId, @source, @eventData, @status, @errorMessage)";

                using var connection = new MySqlConnection(_connectionString);
                using var command = new MySqlCommand(sql, connection);
                
                command.Parameters.AddWithValue("@eventId", eventId);
                command.Parameters.AddWithValue("@eventType", eventType);
                command.Parameters.AddWithValue("@correlationId", correlationId ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@source", source ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@eventData", eventData);
                command.Parameters.AddWithValue("@status", status);
                command.Parameters.AddWithValue("@errorMessage", errorMessage ?? (object)DBNull.Value);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();

                _logger.LogInformation("Event logged: {EventType} with ID {EventId}", eventType, eventId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log event {EventType}", typeof(T).Name);
            }
        }

        /// <summary>
        /// 记录事件处理结果
        /// </summary>
        public async Task LogEventProcessingAsync(string eventId, string handlerType, bool success, int processingTimeMs, string? errorMessage = null)
        {
            try
            {
                var sql = @"
                    INSERT INTO event_processing_history (event_log_id, handler_type, status, processing_time_ms, error_message)
                    SELECT id, @handlerType, @status, @processingTimeMs, @errorMessage
                    FROM event_logs WHERE event_id = @eventId";

                using var connection = new MySqlConnection(_connectionString);
                using var command = new MySqlCommand(sql, connection);
                
                command.Parameters.AddWithValue("@eventId", eventId);
                command.Parameters.AddWithValue("@handlerType", handlerType);
                command.Parameters.AddWithValue("@status", success ? "success" : "failed");
                command.Parameters.AddWithValue("@processingTimeMs", processingTimeMs);
                command.Parameters.AddWithValue("@errorMessage", errorMessage ?? (object)DBNull.Value);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();

                _logger.LogInformation("Event processing logged: {HandlerType} for event {EventId}", handlerType, eventId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log event processing for event {EventId}", eventId);
            }
        }

        /// <summary>
        /// 记录业务操作日志
        /// </summary>
        public async Task LogBusinessOperationAsync(string operationType, string entityType, string entityId, object? operationData = null, string? operatorName = null, string? correlationId = null, bool success = true, string? errorMessage = null)
        {
            try
            {
                var sql = @"
                    INSERT INTO business_operation_logs (operation_type, entity_type, entity_id, operation_data, operator, correlation_id, status, error_message)
                    VALUES (@operationType, @entityType, @entityId, @operationData, @operator, @correlationId, @status, @errorMessage)";

                using var connection = new MySqlConnection(_connectionString);
                using var command = new MySqlCommand(sql, connection);
                
                command.Parameters.AddWithValue("@operationType", operationType);
                command.Parameters.AddWithValue("@entityType", entityType);
                command.Parameters.AddWithValue("@entityId", entityId);
                command.Parameters.AddWithValue("@operationData", operationData != null ? JsonConvert.SerializeObject(operationData) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@operator", operatorName ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@correlationId", correlationId ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@status", success ? "success" : "failed");
                command.Parameters.AddWithValue("@errorMessage", errorMessage ?? (object)DBNull.Value);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();

                _logger.LogInformation("Business operation logged: {OperationType} on {EntityType} {EntityId}", operationType, entityType, entityId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log business operation {OperationType}", operationType);
            }
        }

        /// <summary>
        /// 记录库存变更日志
        /// </summary>
        public async Task LogInventoryChangeAsync(Guid productId, int oldStock, int newStock, string operationType, string reason, Guid? relatedOrderId = null, string? operatorName = null, string? correlationId = null)
        {
            try
            {
                var sql = @"
                    INSERT INTO inventory_change_logs (product_id, old_stock, new_stock, change_amount, operation_type, reason, related_order_id, operator, correlation_id)
                    VALUES (@productId, @oldStock, @newStock, @changeAmount, @operationType, @reason, @relatedOrderId, @operator, @correlationId)";

                using var connection = new MySqlConnection(_connectionString);
                using var command = new MySqlCommand(sql, connection);
                
                command.Parameters.AddWithValue("@productId", productId.ToString());
                command.Parameters.AddWithValue("@oldStock", oldStock);
                command.Parameters.AddWithValue("@newStock", newStock);
                command.Parameters.AddWithValue("@changeAmount", newStock - oldStock);
                command.Parameters.AddWithValue("@operationType", operationType);
                command.Parameters.AddWithValue("@reason", reason);
                command.Parameters.AddWithValue("@relatedOrderId", relatedOrderId?.ToString() ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@operator", operatorName ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@correlationId", correlationId ?? (object)DBNull.Value);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();

                _logger.LogInformation("Inventory change logged: {OperationType} for product {ProductId}, stock changed from {OldStock} to {NewStock}", 
                    operationType, productId, oldStock, newStock);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log inventory change for product {ProductId}", productId);
            }
        }

        /// <summary>
        /// 记录订单状态变更日志
        /// </summary>
        public async Task LogOrderStatusChangeAsync(Guid orderId, string oldStatus, string newStatus, string? reason = null, string? operatorName = null, object? statusData = null, string? correlationId = null)
        {
            try
            {
                var sql = @"
                    INSERT INTO order_status_change_logs (order_id, old_status, new_status, reason, operator, status_data, correlation_id)
                    VALUES (@orderId, @oldStatus, @newStatus, @reason, @operator, @statusData, @correlationId)";

                using var connection = new MySqlConnection(_connectionString);
                using var command = new MySqlCommand(sql, connection);
                
                command.Parameters.AddWithValue("@orderId", orderId.ToString());
                command.Parameters.AddWithValue("@oldStatus", oldStatus);
                command.Parameters.AddWithValue("@newStatus", newStatus);
                command.Parameters.AddWithValue("@reason", reason ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@operator", operatorName ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@statusData", statusData != null ? JsonConvert.SerializeObject(statusData) : (object)DBNull.Value);
                command.Parameters.AddWithValue("@correlationId", correlationId ?? (object)DBNull.Value);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();

                _logger.LogInformation("Order status change logged: {OrderId} changed from {OldStatus} to {NewStatus}", 
                    orderId, oldStatus, newStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log order status change for order {OrderId}", orderId);
            }
        }

        /// <summary>
        /// 记录支付处理日志
        /// </summary>
        public async Task LogPaymentProcessingAsync(string paymentId, Guid orderId, decimal amount, string paymentMethod, string status, string? transactionId = null, string? errorMessage = null, int? processingTimeMs = null, string? correlationId = null)
        {
            try
            {
                var sql = @"
                    INSERT INTO payment_processing_logs (payment_id, order_id, amount, payment_method, status, transaction_id, error_message, processing_time_ms, correlation_id)
                    VALUES (@paymentId, @orderId, @amount, @paymentMethod, @status, @transactionId, @errorMessage, @processingTimeMs, @correlationId)";

                using var connection = new MySqlConnection(_connectionString);
                using var command = new MySqlCommand(sql, connection);
                
                command.Parameters.AddWithValue("@paymentId", paymentId);
                command.Parameters.AddWithValue("@orderId", orderId.ToString());
                command.Parameters.AddWithValue("@amount", amount);
                command.Parameters.AddWithValue("@paymentMethod", paymentMethod);
                command.Parameters.AddWithValue("@status", status);
                command.Parameters.AddWithValue("@transactionId", transactionId ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@errorMessage", errorMessage ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@processingTimeMs", processingTimeMs ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@correlationId", correlationId ?? (object)DBNull.Value);

                await connection.OpenAsync();
                await command.ExecuteNonQueryAsync();

                _logger.LogInformation("Payment processing logged: {PaymentId} for order {OrderId} with status {Status}", 
                    paymentId, orderId, status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log payment processing for payment {PaymentId}", paymentId);
            }
        }

        /// <summary>
        /// 获取事件处理统计
        /// </summary>
        public async Task<object> GetEventProcessingStatsAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var sql = @"
                    SELECT 
                        event_type,
                        status,
                        COUNT(*) as count,
                        AVG(CASE WHEN processing_time_ms IS NOT NULL THEN processing_time_ms END) as avg_processing_time
                    FROM event_logs el
                    LEFT JOIN event_processing_history eph ON el.id = eph.event_log_id
                    WHERE el.created_at BETWEEN @startDate AND @endDate
                    GROUP BY event_type, status";

                using var connection = new MySqlConnection(_connectionString);
                using var command = new MySqlCommand(sql, connection);
                
                command.Parameters.AddWithValue("@startDate", startDate);
                command.Parameters.AddWithValue("@endDate", endDate);

                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                var stats = new List<object>();
                while (await reader.ReadAsync())
                {
                    stats.Add(new
                    {
                        EventType = reader["event_type"],
                        Status = reader["status"],
                        Count = Convert.ToInt32(reader["count"]),
                        AvgProcessingTime = reader["avg_processing_time"] != DBNull.Value ? Convert.ToDouble(reader["avg_processing_time"]) : 0.0
                    });
                }

                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get event processing stats");
                return new List<object>();
            }
        }

        /// <summary>
        /// 获取业务操作统计
        /// </summary>
        public async Task<object> GetBusinessOperationStatsAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var sql = @"
                    SELECT 
                        operation_type,
                        entity_type,
                        status,
                        COUNT(*) as count
                    FROM business_operation_logs
                    WHERE created_at BETWEEN @startDate AND @endDate
                    GROUP BY operation_type, entity_type, status";

                using var connection = new MySqlConnection(_connectionString);
                using var command = new MySqlCommand(sql, connection);
                
                command.Parameters.AddWithValue("@startDate", startDate);
                command.Parameters.AddWithValue("@endDate", endDate);

                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                var stats = new List<object>();
                while (await reader.ReadAsync())
                {
                    stats.Add(new
                    {
                        OperationType = reader["operation_type"],
                        EntityType = reader["entity_type"],
                        Status = reader["status"],
                        Count = Convert.ToInt32(reader["count"])
                    });
                }

                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get business operation stats");
                return new List<object>();
            }
        }

        #region Private Helper Methods

        private string GetEventId(object @event)
        {
            var eventIdProperty = @event.GetType().GetProperty("EventId");
            if (eventIdProperty != null)
            {
                var value = eventIdProperty.GetValue(@event);
                return value?.ToString() ?? Guid.NewGuid().ToString();
            }
            return Guid.NewGuid().ToString();
        }

        private string? GetCorrelationId(object @event)
        {
            var correlationIdProperty = @event.GetType().GetProperty("CorrelationId");
            if (correlationIdProperty != null)
            {
                var value = correlationIdProperty.GetValue(@event);
                return value?.ToString();
            }
            return null;
        }

        private string? GetEventSource(object @event)
        {
            var sourceProperty = @event.GetType().GetProperty("Source");
            if (sourceProperty != null)
            {
                var value = sourceProperty.GetValue(@event);
                return value?.ToString();
            }
            return null;
        }

        #endregion
    }
}

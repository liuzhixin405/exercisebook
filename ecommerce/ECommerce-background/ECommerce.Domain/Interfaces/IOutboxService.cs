using ECommerce.Domain.Models;

namespace ECommerce.Domain.Interfaces
{
    public interface IOutboxService
    {
        /// <summary>
        /// 将事件添加到Outbox，确保事务一致性
        /// </summary>
        /// <param name="domainEvent">领域事件</param>
        /// <param name="correlationId">关联ID</param>
        /// <param name="causationId">原因ID</param>
        Task AddEventAsync(DomainEvent domainEvent, string? correlationId = null, string? causationId = null);

        /// <summary>
        /// 批量添加事件
        /// </summary>
        /// <param name="domainEvents">领域事件集合</param>
        /// <param name="correlationId">关联ID</param>
        /// <param name="causationId">原因ID</param>
        Task AddEventsAsync(IEnumerable<DomainEvent> domainEvents, string? correlationId = null, string? causationId = null);

        /// <summary>
        /// 获取待处理的事件
        /// </summary>
        /// <param name="batchSize">批次大小</param>
        /// <returns>待处理事件列表</returns>
        Task<IEnumerable<OutboxMessage>> GetPendingEventsAsync(int batchSize = 100);

        /// <summary>
        /// 标记事件为处理中
        /// </summary>
        /// <param name="eventId">事件ID</param>
        Task MarkAsProcessingAsync(Guid eventId);

        /// <summary>
        /// 标记事件为已完成
        /// </summary>
        /// <param name="eventId">事件ID</param>
        Task MarkAsCompletedAsync(Guid eventId);

        /// <summary>
        /// 标记事件为失败
        /// </summary>
        /// <summary>
        /// <param name="eventId">事件ID</param>
        /// <param name="error">错误信息</param>
        /// <param name="retryAfter">重试时间</param>
        Task MarkAsFailedAsync(Guid eventId, string error, DateTime? retryAfter = null);

        /// <summary>
        /// 清理已完成的事件
        /// </summary>
        /// <param name="olderThan">清理早于此时间的事件</param>
        Task CleanupCompletedEventsAsync(DateTime olderThan);

        /// <summary>
        /// 获取失败的事件用于重试
        /// </summary>
        /// <param name="batchSize">批次大小</param>
        /// <returns>失败事件列表</returns>
        Task<IEnumerable<OutboxMessage>> GetFailedEventsForRetryAsync(int batchSize = 50);
    }
}

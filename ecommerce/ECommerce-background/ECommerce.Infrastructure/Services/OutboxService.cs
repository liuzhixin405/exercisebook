using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Models;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ECommerce.Infrastructure.Services
{
    public class OutboxService : IOutboxService
    {
        private readonly ECommerceDbContext _context;
        private readonly ILogger<OutboxService> _logger;

        public OutboxService(ECommerceDbContext context, ILogger<OutboxService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddEventAsync(DomainEvent domainEvent, string? correlationId = null, string? causationId = null)
        {
            try
            {
                var outboxMessage = new OutboxMessage
                {
                    Id = domainEvent.Id,
                    EventType = domainEvent.EventType,
                    EventData = domainEvent.Data,
                    CreatedAt = domainEvent.OccurredOn,
                    Status = OutboxMessageStatus.Pending,
                    RetryCount = 0
                };

                _context.OutboxMessages.Add(outboxMessage);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Event {EventType} with ID {EventId} added to outbox", 
                    domainEvent.EventType, domainEvent.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add event {EventType} to outbox", domainEvent.EventType);
                throw;
            }
        }

        public async Task AddEventsAsync(IEnumerable<DomainEvent> domainEvents, string? correlationId = null, string? causationId = null)
        {
            try
            {
                var outboxMessages = domainEvents.Select(domainEvent => new OutboxMessage
                {
                    Id = domainEvent.Id,
                    EventType = domainEvent.EventType,
                    EventData = domainEvent.Data,
                    CreatedAt = domainEvent.OccurredOn,
                    Status = OutboxMessageStatus.Pending,
                    RetryCount = 0
                }).ToList();

                _context.OutboxMessages.AddRange(outboxMessages);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Added {Count} events to outbox", outboxMessages.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add events to outbox");
                throw;
            }
        }

        public async Task<IEnumerable<OutboxMessage>> GetPendingEventsAsync(int batchSize = 100)
        {
            try
            {
                var events = await _context.OutboxMessages
                    .Where(e => e.Status == OutboxMessageStatus.Pending)
                    .OrderBy(e => e.CreatedAt)
                    .Take(batchSize)
                    .ToListAsync();

                _logger.LogDebug("Retrieved {Count} pending events from outbox", events.Count);
                return events;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve pending events from outbox");
                throw;
            }
        }

        public async Task MarkAsProcessingAsync(Guid eventId)
        {
            try
            {
                var outboxMessage = await _context.OutboxMessages.FindAsync(eventId);
                if (outboxMessage != null)
                {
                    outboxMessage.Status = OutboxMessageStatus.Processing;
                    await _context.SaveChangesAsync();

                    _logger.LogDebug("Marked event {EventId} as processing", eventId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to mark event {EventId} as processing", eventId);
                throw;
            }
        }

        public async Task MarkAsCompletedAsync(Guid eventId)
        {
            try
            {
                var outboxMessage = await _context.OutboxMessages.FindAsync(eventId);
                if (outboxMessage != null)
                {
                    outboxMessage.Status = OutboxMessageStatus.Completed;
                    outboxMessage.ProcessedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();

                    _logger.LogDebug("Marked event {EventId} as completed", eventId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to mark event {EventId} as completed", eventId);
                throw;
            }
        }

        public async Task MarkAsFailedAsync(Guid eventId, string error, DateTime? retryAfter = null)
        {
            try
            {
                var outboxMessage = await _context.OutboxMessages.FindAsync(eventId);
                if (outboxMessage != null)
                {
                    outboxMessage.Status = OutboxMessageStatus.Failed;
                    outboxMessage.ErrorMessage = error;
                    outboxMessage.RetryCount++;
                    
                    if (retryAfter.HasValue)
                    {
                        outboxMessage.Status = OutboxMessageStatus.Pending; // 使用Pending状态表示需要重试
                        outboxMessage.ProcessedAt = retryAfter.Value;
                    }

                    await _context.SaveChangesAsync();

                    _logger.LogWarning("Marked event {EventId} as failed: {Error}", eventId, error);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to mark event {EventId} as failed", eventId);
                throw;
            }
        }

        public async Task CleanupCompletedEventsAsync(DateTime olderThan)
        {
            try
            {
                var completedEvents = await _context.OutboxMessages
                    .Where(e => e.Status == OutboxMessageStatus.Completed && e.ProcessedAt < olderThan)
                    .ToListAsync();

                if (completedEvents.Any())
                {
                    _context.OutboxMessages.RemoveRange(completedEvents);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Cleaned up {Count} completed events older than {OlderThan}", 
                        completedEvents.Count, olderThan);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cleanup completed events");
                throw;
            }
        }

        public async Task<IEnumerable<OutboxMessage>> GetFailedEventsForRetryAsync(int batchSize = 50)
        {
            try
            {
                var now = DateTime.UtcNow;
                var failedEvents = await _context.OutboxMessages
                    .Where(e => e.Status == OutboxMessageStatus.Failed && 
                               e.RetryCount < 3) // 最多重试3次
                    .OrderBy(e => e.CreatedAt)
                    .Take(batchSize)
                    .ToListAsync();

                _logger.LogDebug("Retrieved {Count} failed events for retry", failedEvents.Count);
                return failedEvents;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve failed events for retry");
                throw;
            }
        }
    }
}

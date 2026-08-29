using AgenticCommerceErpDemo.Application.Auditing;
using AgenticCommerceErpDemo.Application.Business;

namespace AgenticCommerceErpDemo.Infrastructure.Services;

public sealed class NotificationService(IAuditLog audit) : INotificationService
{
    public object NotifyOps(string title, string message)
    {
        var notification = new { Id = $"msg-{Guid.NewGuid():n}", title, message, SentAt = DateTimeOffset.UtcNow };
        audit.Write("notification.sent", "OpsAgent", notification);
        return notification;
    }
}

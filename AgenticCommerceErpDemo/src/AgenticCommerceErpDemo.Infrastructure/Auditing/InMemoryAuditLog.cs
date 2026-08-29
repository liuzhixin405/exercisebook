using System.Collections.Concurrent;
using AgenticCommerceErpDemo.Application.Auditing;

namespace AgenticCommerceErpDemo.Infrastructure.Auditing;

public sealed class InMemoryAuditLog : IAuditLog
{
    private readonly ConcurrentQueue<AuditEvent> _events = new();

    public IReadOnlyList<AuditEvent> Events => _events.ToArray();

    public void Write(string type, string actor, object payload)
    {
        _events.Enqueue(new AuditEvent(Guid.NewGuid().ToString("n"), DateTimeOffset.UtcNow, type, actor, payload));
    }
}

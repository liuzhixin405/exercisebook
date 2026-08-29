namespace AgenticCommerceErpDemo.Application.Auditing;

public sealed record AuditEvent(string Id, DateTimeOffset At, string Type, string Actor, object Payload);

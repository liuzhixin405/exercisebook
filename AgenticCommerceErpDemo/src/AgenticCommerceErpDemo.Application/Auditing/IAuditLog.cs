namespace AgenticCommerceErpDemo.Application.Auditing;

public interface IAuditLog
{
    IReadOnlyList<AuditEvent> Events { get; }
    void Write(string type, string actor, object payload);
}

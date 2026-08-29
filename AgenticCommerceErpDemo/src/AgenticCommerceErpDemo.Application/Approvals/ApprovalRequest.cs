using AgenticCommerceErpDemo.Domain.Common;

namespace AgenticCommerceErpDemo.Application.Approvals;

public sealed record ApprovalRequest(
    string Id,
    string ActionName,
    RiskLevel Risk,
    object Payload,
    ApprovalStatus Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? DecidedAt = null)
{
    public ApprovalStatus Status { get; set; } = Status;
    public DateTimeOffset? DecidedAt { get; set; } = DecidedAt;
}

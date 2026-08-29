using AgenticCommerceErpDemo.Domain.Common;

namespace AgenticCommerceErpDemo.Application.Approvals;

public interface IApprovalRepository
{
    ApprovalRequest? Find(string approvalId);
    IReadOnlyList<ApprovalRequest> ListPending();
    void Add(ApprovalRequest approval);
    void MarkDecision(string approvalId, ApprovalStatus status);
}

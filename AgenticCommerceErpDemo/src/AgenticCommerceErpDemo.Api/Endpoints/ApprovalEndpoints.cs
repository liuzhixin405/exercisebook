using AgenticCommerceErpDemo.Application.Approvals;
using AgenticCommerceErpDemo.Application.Auditing;
using AgenticCommerceErpDemo.Domain.Common;

namespace AgenticCommerceErpDemo.Api.Endpoints;

public static class ApprovalEndpoints
{
    public static IEndpointRouteBuilder MapApprovalEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/approvals/{approvalId}/approve", (string approvalId, IApprovalRepository approvals, IAuditLog audit) =>
        {
            return Decide(approvalId, ApprovalStatus.Approved, approvals, audit);
        });

        app.MapPost("/approvals/{approvalId}/reject", (string approvalId, IApprovalRepository approvals, IAuditLog audit) =>
        {
            return Decide(approvalId, ApprovalStatus.Rejected, approvals, audit);
        });

        return app;
    }

    private static IResult Decide(string approvalId, ApprovalStatus status, IApprovalRepository approvals, IAuditLog audit)
    {
        var approval = approvals.Find(approvalId);
        if (approval is null)
        {
            return Results.NotFound(new { message = "Approval not found." });
        }

        if (approval.Status != ApprovalStatus.Pending)
        {
            return Results.Conflict(new { message = $"Approval is already {approval.Status}." });
        }

        approvals.MarkDecision(approvalId, status);
        audit.Write($"approval.{status.ToString().ToLowerInvariant()}", "human", new { approval.Id, approval.ActionName, approval.Payload });
        return Results.Ok(approval);
    }
}

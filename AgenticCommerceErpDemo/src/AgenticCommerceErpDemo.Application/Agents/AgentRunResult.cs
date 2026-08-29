namespace AgenticCommerceErpDemo.Application.Agents;

public sealed record AgentRunResult(
    string RunId,
    string Goal,
    IReadOnlyList<AgentStepResult> Steps,
    IReadOnlyList<ApprovalRequest> PendingApprovals,
    string ExecutiveSummary);

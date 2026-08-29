using AgenticCommerceErpDemo.Domain.Common;

namespace AgenticCommerceErpDemo.Application.Agents;

public sealed record AgentStepResult(
    string Agent,
    string ToolName,
    RiskLevel Risk,
    string Status,
    object? Output,
    string? ApprovalId = null);

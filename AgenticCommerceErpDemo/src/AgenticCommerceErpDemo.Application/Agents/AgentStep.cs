using AgenticCommerceErpDemo.Domain.Common;

namespace AgenticCommerceErpDemo.Application.Agents;

public sealed record AgentStep(string Agent, string ToolName, Dictionary<string, object?> Arguments, RiskLevel Risk);

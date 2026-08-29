namespace AgenticCommerceErpDemo.Application.Agents;

public sealed record AgentPlan(string Intent, IReadOnlyList<AgentStep> Steps);

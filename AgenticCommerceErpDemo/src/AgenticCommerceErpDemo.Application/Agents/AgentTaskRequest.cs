namespace AgenticCommerceErpDemo.Application.Agents;

public sealed record AgentTaskRequest(string Goal, string? TenantId = "demo-tenant", string? UserId = "ops-user");

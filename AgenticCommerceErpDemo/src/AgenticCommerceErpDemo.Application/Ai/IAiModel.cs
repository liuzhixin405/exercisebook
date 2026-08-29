using AgenticCommerceErpDemo.Application.Agents;

namespace AgenticCommerceErpDemo.Application.Ai;

public interface IAiModel
{
    Task<AgentPlan> CreatePlanAsync(string goal, CancellationToken ct);
    Task<string> SummarizeAsync(string goal, IReadOnlyList<AgentStepResult> results, CancellationToken ct);
}

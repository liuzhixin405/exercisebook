using AgenticCommerceErpDemo.Application.Agents;
using AgenticCommerceErpDemo.Application.Ai;
using AgenticCommerceErpDemo.Domain.Common;

namespace AgenticCommerceErpDemo.Infrastructure.Ai;

public sealed class DeterministicAiModel : IAiModel
{
    public Task<AgentPlan> CreatePlanAsync(string goal, CancellationToken ct)
    {
        var normalized = goal.ToLowerInvariant();
        var steps = new List<AgentStep>
        {
            new("KnowledgeAgent", "knowledge.search", Args(("query", goal)), RiskLevel.Low)
        };

        if (normalized.Contains("inventory") || normalized.Contains("stock") || normalized.Contains("warehouse") || normalized.Contains("erp"))
        {
            steps.Add(new("InventoryAgent", "inventory.analyzeRisk", Args(("warehouseCode", "EAST")), RiskLevel.Low));
        }

        if (normalized.Contains("conversion") || normalized.Contains("commerce") || normalized.Contains("product") || normalized.Contains("promotion"))
        {
            steps.Add(new("CommerceAgent", "catalog.analyzeConversionDrops", Args(), RiskLevel.Low));
            steps.Add(new("CustomerAgent", "customer.summarizeComplaints", Args(("skus", new[] { "SKU-1001", "SKU-2002", "SKU-3003" })), RiskLevel.Low));
            steps.Add(new("FinanceAgent", "finance.checkRisk", Args(("plannedSpend", 16_000m), ("plannedDiscountPercent", 10m)), RiskLevel.Medium));
            steps.Add(new("CommerceAgent", "commerce.createPromotionDraft", Args(("sku", "SKU-2002"), ("discountPercent", 10m), ("reason", "Recover conversion while preserving margin.")), RiskLevel.Medium));
        }

        if (normalized.Contains("replenish") || normalized.Contains("purchase") || normalized.Contains("补货") || normalized.Contains("采购"))
        {
            steps.Add(new("InventoryAgent", "inventory.createPurchaseOrderDraft", Args(("supplierCode", "SUP-88"), ("skus", new[] { "SKU-1001", "SKU-2002" })), RiskLevel.High));
        }

        steps.Add(new("OpsAgent", "ops.notify", Args(
            ("title", "Agent workflow completed"),
            ("message", "Review generated drafts and pending approvals.")), RiskLevel.Low));

        return Task.FromResult(new AgentPlan("mixed-commerce-erp-operations", steps));
    }

    public Task<string> SummarizeAsync(string goal, IReadOnlyList<AgentStepResult> results, CancellationToken ct)
    {
        var completed = results.Count(x => x.Status == "completed");
        var pending = results.Count(x => x.Status == "pending_approval");
        var failed = results.Count(x => x.Status == "failed");
        var summary = $"Goal processed with {completed} completed steps, {pending} pending approvals, and {failed} failed steps. " +
                      "Low-risk analysis and drafts were produced through business tools; high-risk write actions were routed to human approval.";
        return Task.FromResult(summary);
    }

    private static Dictionary<string, object?> Args(params (string Name, object? Value)[] values)
    {
        return values.ToDictionary(x => x.Name, x => x.Value);
    }
}

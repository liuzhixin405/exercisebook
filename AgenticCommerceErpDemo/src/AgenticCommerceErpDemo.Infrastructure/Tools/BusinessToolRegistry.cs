using System.Text.Json;
using AgenticCommerceErpDemo.Application.Business;
using AgenticCommerceErpDemo.Application.Knowledge;
using AgenticCommerceErpDemo.Application.Tools;

namespace AgenticCommerceErpDemo.Infrastructure.Tools;

public sealed class BusinessToolRegistry(
    IInventoryService inventory,
    ICatalogService catalog,
    ICustomerService customer,
    IFinanceService finance,
    IOrderService orders,
    INotificationService notification,
    IKnowledgeBase knowledgeBase) : IBusinessToolRegistry
{
    public object Invoke(string toolName, Dictionary<string, object?> arguments)
    {
        return toolName switch
        {
            "knowledge.search" => knowledgeBase.Search(GetString(arguments, "query")),
            "inventory.analyzeRisk" => inventory.AnalyzeInventoryRisk(GetString(arguments, "warehouseCode")),
            "catalog.analyzeConversionDrops" => catalog.AnalyzeConversionDrops(),
            "customer.summarizeComplaints" => customer.SummarizeComplaints(GetStringList(arguments, "skus")),
            "finance.checkRisk" => finance.CheckRisk(GetDecimal(arguments, "plannedSpend"), GetDecimal(arguments, "plannedDiscountPercent")),
            "inventory.createPurchaseOrderDraft" => inventory.CreatePurchaseOrderDraft(GetString(arguments, "supplierCode"), GetStringList(arguments, "skus")),
            "commerce.createPromotionDraft" => orders.CreatePromotionDraft(GetString(arguments, "sku"), GetDecimal(arguments, "discountPercent"), GetString(arguments, "reason")),
            "ops.notify" => notification.NotifyOps(GetString(arguments, "title"), GetString(arguments, "message")),
            _ => throw new InvalidOperationException($"Unknown tool '{toolName}'.")
        };
    }

    private static string GetString(Dictionary<string, object?> args, string name)
        => args.TryGetValue(name, out var value) ? Convert.ToString(value) ?? "" : "";

    private static decimal GetDecimal(Dictionary<string, object?> args, string name)
    {
        if (!args.TryGetValue(name, out var value) || value is null)
        {
            return 0m;
        }

        if (value is JsonElement json && json.ValueKind == JsonValueKind.Number)
        {
            return json.GetDecimal();
        }

        return Convert.ToDecimal(value);
    }

    private static IReadOnlyList<string> GetStringList(Dictionary<string, object?> args, string name)
    {
        if (!args.TryGetValue(name, out var value) || value is null)
        {
            return [];
        }

        if (value is JsonElement json && json.ValueKind == JsonValueKind.Array)
        {
            return json.EnumerateArray().Select(x => x.GetString() ?? "").Where(x => x.Length > 0).ToList();
        }

        if (value is IEnumerable<string> strings)
        {
            return strings.ToList();
        }

        return Convert.ToString(value)?
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList() ?? [];
    }
}

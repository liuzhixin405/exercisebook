using AgenticCommerceErpDemo.Application.Auditing;
using AgenticCommerceErpDemo.Application.Business;
using AgenticCommerceErpDemo.Domain.Procurement;
using AgenticCommerceErpDemo.Infrastructure.Persistence;

namespace AgenticCommerceErpDemo.Infrastructure.Services;

public sealed class InventoryService(InMemoryAppDataStore store, IAuditLog audit) : IInventoryService
{
    public IReadOnlyList<object> AnalyzeInventoryRisk(string warehouseCode)
    {
        var report = store.Inventory
            .Where(x => x.WarehouseCode.Equals(warehouseCode, StringComparison.OrdinalIgnoreCase))
            .Join(store.Forecasts, inv => inv.Sku, forecast => forecast.Sku, (inv, forecast) =>
            {
                var available = inv.OnHand - inv.Reserved;
                var projectedGap = forecast.ExpectedUnitsNext14Days + inv.SafetyStock - available;
                return new
                {
                    inv.Sku,
                    Available = available,
                    inv.SafetyStock,
                    forecast.ExpectedUnitsNext14Days,
                    forecast.Confidence,
                    ReorderQuantity = Math.Max(0, projectedGap),
                    Severity = projectedGap switch
                    {
                        > 120 => "critical",
                        > 50 => "high",
                        > 0 => "medium",
                        _ => "healthy"
                    }
                };
            })
            .OrderByDescending(x => x.ReorderQuantity)
            .Cast<object>()
            .ToList();

        audit.Write("inventory.risk.analyzed", "InventoryAgent", new { warehouseCode, count = report.Count });
        return report;
    }

    public PurchaseOrderDraft CreatePurchaseOrderDraft(string supplierCode, IReadOnlyList<string> skus)
    {
        var lines = skus
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(sku =>
            {
                var inventory = store.Inventory.First(x => x.Sku == sku);
                var forecast = store.Forecasts.First(x => x.Sku == sku);
                var product = store.Products.First(x => x.Sku == sku);
                var available = inventory.OnHand - inventory.Reserved;
                var quantity = Math.Max(20, forecast.ExpectedUnitsNext14Days + inventory.SafetyStock - available);
                return new PurchaseOrderLine(sku, quantity, product.Cost);
            })
            .Where(x => x.Quantity > 0)
            .ToList();

        var draft = new PurchaseOrderDraft(
            $"po-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}",
            supplierCode,
            lines,
            lines.Sum(x => x.Quantity * x.UnitCost));

        store.PurchaseOrders.Add(draft);
        audit.Write("purchase_order.draft.created", "InventoryAgent", draft);
        return draft;
    }
}

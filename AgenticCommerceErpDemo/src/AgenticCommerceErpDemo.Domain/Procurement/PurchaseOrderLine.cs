namespace AgenticCommerceErpDemo.Domain.Procurement;

public sealed record PurchaseOrderLine(string Sku, int Quantity, decimal UnitCost);

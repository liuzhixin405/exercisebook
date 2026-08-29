namespace AgenticCommerceErpDemo.Domain.Procurement;

public sealed record PurchaseOrderDraft(string Id, string SupplierCode, IReadOnlyList<PurchaseOrderLine> Lines, decimal EstimatedTotal);

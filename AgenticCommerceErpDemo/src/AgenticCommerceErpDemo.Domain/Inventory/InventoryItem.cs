namespace AgenticCommerceErpDemo.Domain.Inventory;

public sealed record InventoryItem(
    string Sku,
    string WarehouseCode,
    int OnHand,
    int Reserved,
    int SafetyStock,
    int LeadTimeDays);

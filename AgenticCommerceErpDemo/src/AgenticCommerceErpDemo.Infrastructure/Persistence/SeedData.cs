using AgenticCommerceErpDemo.Domain.Catalog;
using AgenticCommerceErpDemo.Domain.Customer;
using AgenticCommerceErpDemo.Domain.Inventory;

namespace AgenticCommerceErpDemo.Infrastructure.Persistence;

internal static class SeedData
{
    public static List<Product> Products() =>
    [
        new("SKU-1001", "Noise Cancelling Headphones", "Electronics", 199m, 118m, 0.041, 0.067, 18500, 0.031),
        new("SKU-2002", "Smart Fitness Band", "Electronics", 89m, 42m, 0.026, 0.058, 23300, 0.062),
        new("SKU-3003", "Ergonomic Office Chair", "Office", 249m, 151m, 0.034, 0.039, 4900, 0.044),
        new("SKU-4004", "Standing Desk Converter", "Office", 139m, 82m, 0.049, 0.047, 3700, 0.025)
    ];

    public static List<InventoryItem> Inventory() =>
    [
        new("SKU-1001", "EAST", 95, 41, 120, 12),
        new("SKU-2002", "EAST", 68, 18, 180, 9),
        new("SKU-3003", "EAST", 220, 30, 90, 16),
        new("SKU-4004", "EAST", 155, 22, 80, 14),
        new("SKU-1001", "WEST", 260, 80, 100, 10)
    ];

    public static List<SalesForecast> Forecasts() =>
    [
        new("SKU-1001", 155, 0.82),
        new("SKU-2002", 240, 0.78),
        new("SKU-3003", 70, 0.71),
        new("SKU-4004", 95, 0.69)
    ];

    public static List<CustomerComplaint> Complaints() =>
    [
        new("c-001", "SKU-2002", "marketplace", "battery drains quickly after firmware update", DateTimeOffset.UtcNow.AddDays(-2)),
        new("c-002", "SKU-2002", "support", "battery life is much shorter than product page says", DateTimeOffset.UtcNow.AddDays(-1)),
        new("c-003", "SKU-1001", "support", "package arrived damaged and the case was missing", DateTimeOffset.UtcNow.AddDays(-3)),
        new("c-004", "SKU-1001", "marketplace", "bluetooth pairing instructions were unclear", DateTimeOffset.UtcNow.AddDays(-4)),
        new("c-005", "SKU-3003", "support", "chair size is too large for small desks", DateTimeOffset.UtcNow.AddDays(-2))
    ];
}

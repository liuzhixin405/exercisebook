namespace AgenticCommerceErpDemo.Domain.Inventory;

public sealed record SalesForecast(string Sku, int ExpectedUnitsNext14Days, double Confidence);

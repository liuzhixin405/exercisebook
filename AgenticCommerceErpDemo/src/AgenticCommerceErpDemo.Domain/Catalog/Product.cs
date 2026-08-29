namespace AgenticCommerceErpDemo.Domain.Catalog;

public sealed record Product(
    string Sku,
    string Name,
    string Category,
    decimal Price,
    decimal Cost,
    double ConversionRate,
    double ConversionRateSevenDaysAgo,
    int ViewsLast7Days,
    double ReturnRate);

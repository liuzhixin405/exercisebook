using AgenticCommerceErpDemo.Application.Auditing;
using AgenticCommerceErpDemo.Application.Business;
using AgenticCommerceErpDemo.Infrastructure.Persistence;

namespace AgenticCommerceErpDemo.Infrastructure.Services;

public sealed class CatalogService(InMemoryAppDataStore store, IAuditLog audit) : ICatalogService
{
    public IReadOnlyList<object> AnalyzeConversionDrops()
    {
        var report = store.Products
            .Select(product => new
            {
                product.Sku,
                product.Name,
                product.Category,
                product.ViewsLast7Days,
                product.ConversionRate,
                product.ConversionRateSevenDaysAgo,
                Drop = Math.Round(product.ConversionRateSevenDaysAgo - product.ConversionRate, 4),
                Margin = Math.Round((product.Price - product.Cost) / product.Price, 4)
            })
            .Where(x => x.Drop > 0.015 && x.ViewsLast7Days > 2000)
            .OrderByDescending(x => x.Drop)
            .Cast<object>()
            .ToList();

        audit.Write("catalog.conversion.analyzed", "CommerceAgent", new { count = report.Count });
        return report;
    }
}

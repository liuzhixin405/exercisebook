using AgenticCommerceErpDemo.Application.Auditing;
using AgenticCommerceErpDemo.Application.Business;
using AgenticCommerceErpDemo.Infrastructure.Persistence;

namespace AgenticCommerceErpDemo.Infrastructure.Services;

public sealed class CustomerService(InMemoryAppDataStore store, IAuditLog audit) : ICustomerService
{
    public IReadOnlyList<object> SummarizeComplaints(IReadOnlyList<string> skus)
    {
        var skuSet = skus.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var summary = store.Complaints
            .Where(x => skuSet.Contains(x.Sku))
            .GroupBy(x => x.Sku)
            .Select(group => new
            {
                Sku = group.Key,
                Count = group.Count(),
                TopSignals = group
                    .SelectMany(x => x.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                    .Where(x => x.Length > 5)
                    .GroupBy(x => x.ToLowerInvariant())
                    .OrderByDescending(x => x.Count())
                    .Take(5)
                    .Select(x => x.Key)
                    .ToList(),
                Samples = group.Take(2).Select(x => x.Text).ToList()
            })
            .Cast<object>()
            .ToList();

        audit.Write("customer.complaints.summarized", "CustomerAgent", new { skus, count = summary.Count });
        return summary;
    }
}

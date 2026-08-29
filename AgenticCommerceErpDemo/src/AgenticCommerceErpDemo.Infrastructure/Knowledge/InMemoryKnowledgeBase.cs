using AgenticCommerceErpDemo.Application.Knowledge;
using AgenticCommerceErpDemo.Domain.Knowledge;

namespace AgenticCommerceErpDemo.Infrastructure.Knowledge;

public sealed class InMemoryKnowledgeBase : IKnowledgeBase
{
    private readonly List<KnowledgeDocument> _documents =
    [
        new("kb-001", "East warehouse policy", "East warehouse must keep 14 days of stock for high velocity SKUs. Purchase orders over 10000 require finance review.", ["inventory", "erp", "east"]),
        new("kb-002", "Promotion governance", "Promotion discounts over 12 percent require manager approval. Margin must stay above 20 percent.", ["commerce", "promotion", "risk"]),
        new("kb-003", "Complaint classification", "Repeated comments about battery, size, or damaged package should be treated as product quality or fulfillment risk.", ["customer", "complaint"])
    ];

    public IReadOnlyList<KnowledgeDocument> Search(string query)
    {
        var terms = query.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(x => x.ToLowerInvariant())
            .ToArray();

        return _documents
            .Select(doc => new
            {
                Document = doc,
                Score = terms.Count(term =>
                    doc.Title.Contains(term, StringComparison.OrdinalIgnoreCase)
                    || doc.Content.Contains(term, StringComparison.OrdinalIgnoreCase)
                    || doc.Tags.Any(tag => tag.Contains(term, StringComparison.OrdinalIgnoreCase)))
            })
            .Where(x => x.Score > 0)
            .OrderByDescending(x => x.Score)
            .Select(x => x.Document)
            .Take(5)
            .ToList();
    }
}

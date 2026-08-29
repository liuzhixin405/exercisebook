using AgenticCommerceErpDemo.Application.Auditing;
using AgenticCommerceErpDemo.Application.Business;
using AgenticCommerceErpDemo.Domain.Promotions;
using AgenticCommerceErpDemo.Infrastructure.Persistence;

namespace AgenticCommerceErpDemo.Infrastructure.Services;

public sealed class OrderService(InMemoryAppDataStore store, IAuditLog audit) : IOrderService
{
    public PromotionDraft CreatePromotionDraft(string sku, decimal discountPercent, string reason)
    {
        var product = store.Products.First(x => x.Sku == sku);
        var draft = new PromotionDraft(
            $"promo-{DateTimeOffset.UtcNow:yyyyMMddHHmmss}",
            sku,
            $"Recover conversion for {product.Name}",
            discountPercent,
            reason);

        store.Promotions.Add(draft);
        audit.Write("promotion.draft.created", "CommerceAgent", draft);
        return draft;
    }
}

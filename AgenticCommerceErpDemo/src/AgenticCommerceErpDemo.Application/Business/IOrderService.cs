using AgenticCommerceErpDemo.Domain.Promotions;

namespace AgenticCommerceErpDemo.Application.Business;

public interface IOrderService
{
    PromotionDraft CreatePromotionDraft(string sku, decimal discountPercent, string reason);
}

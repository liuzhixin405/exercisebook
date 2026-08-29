namespace AgenticCommerceErpDemo.Domain.Promotions;

public sealed record PromotionDraft(string Id, string Sku, string Title, decimal SuggestedDiscountPercent, string Reason);

namespace AgenticCommerceErpDemo.Domain.Finance;

public sealed record FinanceRiskReport(decimal OpenReceivables, decimal PromotionBudgetRemaining, bool BudgetAllowsPromotion);

using AgenticCommerceErpDemo.Application.Auditing;
using AgenticCommerceErpDemo.Application.Business;
using AgenticCommerceErpDemo.Domain.Finance;

namespace AgenticCommerceErpDemo.Infrastructure.Services;

public sealed class FinanceService(IAuditLog audit) : IFinanceService
{
    public FinanceRiskReport CheckRisk(decimal plannedSpend, decimal plannedDiscountPercent)
    {
        var report = new FinanceRiskReport(
            OpenReceivables: 183_000m,
            PromotionBudgetRemaining: 25_000m,
            BudgetAllowsPromotion: plannedSpend < 25_000m && plannedDiscountPercent <= 12m);

        audit.Write("finance.risk.checked", "FinanceAgent", new { plannedSpend, plannedDiscountPercent, report });
        return report;
    }
}

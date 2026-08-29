using AgenticCommerceErpDemo.Domain.Finance;

namespace AgenticCommerceErpDemo.Application.Business;

public interface IFinanceService
{
    FinanceRiskReport CheckRisk(decimal plannedSpend, decimal plannedDiscountPercent);
}

using AgenticCommerceErpDemo.Application.Agents;
using AgenticCommerceErpDemo.Application.Guardrails;
using AgenticCommerceErpDemo.Domain.Common;
using AgenticCommerceErpDemo.Domain.Procurement;
using AgenticCommerceErpDemo.Domain.Promotions;

namespace AgenticCommerceErpDemo.Infrastructure.Guardrails;

public sealed class GuardrailPolicy : IGuardrailPolicy
{
    public bool RequiresHumanApproval(AgentStep step, object output)
    {
        if (step.Risk == RiskLevel.High)
        {
            return true;
        }

        if (output is PurchaseOrderDraft po && po.EstimatedTotal > 10_000m)
        {
            return true;
        }

        if (output is PromotionDraft promo && promo.SuggestedDiscountPercent > 12m)
        {
            return true;
        }

        return false;
    }

    public string Explain(AgentStep step, object output)
    {
        return output switch
        {
            PurchaseOrderDraft po when po.EstimatedTotal > 10_000m => $"Purchase order total {po.EstimatedTotal:C} exceeds auto-approval limit.",
            PromotionDraft promo when promo.SuggestedDiscountPercent > 12m => $"Discount {promo.SuggestedDiscountPercent}% exceeds promotion policy.",
            _ when step.Risk == RiskLevel.High => "The action is classified as high risk.",
            _ => "No approval required."
        };
    }
}

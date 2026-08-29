using AgenticCommerceErpDemo.Application.Agents;

namespace AgenticCommerceErpDemo.Application.Guardrails;

public interface IGuardrailPolicy
{
    bool RequiresHumanApproval(AgentStep step, object output);
    string Explain(AgentStep step, object output);
}

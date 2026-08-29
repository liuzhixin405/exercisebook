using AgenticCommerceErpDemo.Application.Ai;
using AgenticCommerceErpDemo.Application.Approvals;
using AgenticCommerceErpDemo.Application.Auditing;
using AgenticCommerceErpDemo.Application.Guardrails;
using AgenticCommerceErpDemo.Application.Tools;
using AgenticCommerceErpDemo.Domain.Common;

namespace AgenticCommerceErpDemo.Application.Agents;

public sealed class AgentOrchestrator(
    IAiModel ai,
    IBusinessToolRegistry tools,
    IGuardrailPolicy guardrails,
    IApprovalRepository approvals,
    IAuditLog audit)
{
    public async Task<AgentRunResult> ExecuteAsync(AgentTaskRequest request, CancellationToken ct)
    {
        var runId = $"run-{Guid.NewGuid():n}";
        audit.Write("agent.run.started", "orchestrator", new { runId, request.Goal, request.TenantId, request.UserId });

        var plan = await ai.CreatePlanAsync(request.Goal, ct);
        audit.Write("agent.plan.created", "orchestrator", new { runId, plan });

        var results = new List<AgentStepResult>();
        foreach (var step in plan.Steps)
        {
            ct.ThrowIfCancellationRequested();
            audit.Write("agent.step.started", step.Agent, new { runId, step.ToolName, step.Arguments, step.Risk });

            object output;
            try
            {
                output = tools.Invoke(step.ToolName, step.Arguments);
            }
            catch (Exception ex)
            {
                audit.Write("agent.step.failed", step.Agent, new { runId, step.ToolName, error = ex.Message });
                results.Add(new AgentStepResult(step.Agent, step.ToolName, step.Risk, "failed", new { ex.Message }));
                continue;
            }

            if (guardrails.RequiresHumanApproval(step, output))
            {
                var approval = new ApprovalRequest(
                    $"approval-{Guid.NewGuid():n}",
                    step.ToolName,
                    step.Risk,
                    new { output, reason = guardrails.Explain(step, output) },
                    ApprovalStatus.Pending,
                    DateTimeOffset.UtcNow);

                approvals.Add(approval);
                audit.Write("agent.step.pending_approval", step.Agent, new { runId, approval.Id, step.ToolName });
                results.Add(new AgentStepResult(step.Agent, step.ToolName, step.Risk, "pending_approval", output, approval.Id));
                continue;
            }

            audit.Write("agent.step.completed", step.Agent, new { runId, step.ToolName });
            results.Add(new AgentStepResult(step.Agent, step.ToolName, step.Risk, "completed", output));
        }

        var summary = await ai.SummarizeAsync(request.Goal, results, ct);
        audit.Write("agent.run.completed", "orchestrator", new { runId, summary });

        return new AgentRunResult(runId, request.Goal, results, approvals.ListPending(), summary);
    }
}

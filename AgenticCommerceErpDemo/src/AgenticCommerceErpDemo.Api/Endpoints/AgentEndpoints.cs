using AgenticCommerceErpDemo.Application.Agents;

namespace AgenticCommerceErpDemo.Api.Endpoints;

public static class AgentEndpoints
{
    public static IEndpointRouteBuilder MapAgentEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/agent/tasks", async (AgentTaskRequest request, AgentOrchestrator orchestrator, CancellationToken ct) =>
        {
            var result = await orchestrator.ExecuteAsync(request, ct);
            return Results.Ok(result);
        });

        return app;
    }
}

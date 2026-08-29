namespace AgenticCommerceErpDemo.Api.Endpoints;

public static class DemoEndpoints
{
    public static IEndpointRouteBuilder MapDemoEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/demo", () => Results.Text("""
Agentic Commerce + ERP Demo

Try:
POST /agent/tasks
{
  "goal": "Analyze East warehouse inventory risk, declining conversion products, customer complaints, and prepare replenishment plus promotion actions."
}

POST /approvals/{approvalId}/approve
GET /audit
GET /state
""", "text/plain"));

        return app;
    }
}

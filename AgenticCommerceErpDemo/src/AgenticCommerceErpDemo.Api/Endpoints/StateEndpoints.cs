using AgenticCommerceErpDemo.Application.Auditing;
using AgenticCommerceErpDemo.Application.State;

namespace AgenticCommerceErpDemo.Api.Endpoints;

public static class StateEndpoints
{
    public static IEndpointRouteBuilder MapStateEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/audit", (IAuditLog audit) => Results.Ok(audit.Events));
        app.MapGet("/state", (IStateReader state) => Results.Ok(state.Snapshot()));
        return app;
    }
}

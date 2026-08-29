using AgenticCommerceErpDemo.Api.Endpoints;
using AgenticCommerceErpDemo.Application.Agents;
using AgenticCommerceErpDemo.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<AgentOrchestrator>();
builder.Services.AddInfrastructure();

var app = builder.Build();

app.MapGet("/", () => Results.Redirect("/demo"));
app.MapDemoEndpoints();
app.MapAgentEndpoints();
app.MapApprovalEndpoints();
app.MapStateEndpoints();

app.Run();

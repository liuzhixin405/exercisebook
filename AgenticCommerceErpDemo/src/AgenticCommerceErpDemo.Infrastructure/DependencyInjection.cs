using AgenticCommerceErpDemo.Application.Ai;
using AgenticCommerceErpDemo.Application.Approvals;
using AgenticCommerceErpDemo.Application.Auditing;
using AgenticCommerceErpDemo.Application.Business;
using AgenticCommerceErpDemo.Application.Guardrails;
using AgenticCommerceErpDemo.Application.Knowledge;
using AgenticCommerceErpDemo.Application.State;
using AgenticCommerceErpDemo.Application.Tools;
using AgenticCommerceErpDemo.Infrastructure.Ai;
using AgenticCommerceErpDemo.Infrastructure.Auditing;
using AgenticCommerceErpDemo.Infrastructure.Guardrails;
using AgenticCommerceErpDemo.Infrastructure.Knowledge;
using AgenticCommerceErpDemo.Infrastructure.Persistence;
using AgenticCommerceErpDemo.Infrastructure.Services;
using AgenticCommerceErpDemo.Infrastructure.Tools;
using Microsoft.Extensions.DependencyInjection;

namespace AgenticCommerceErpDemo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<InMemoryAppDataStore>();
        services.AddSingleton<IApprovalRepository>(sp => sp.GetRequiredService<InMemoryAppDataStore>());
        services.AddSingleton<IStateReader>(sp => sp.GetRequiredService<InMemoryAppDataStore>());

        services.AddSingleton<IAuditLog, InMemoryAuditLog>();
        services.AddSingleton<IKnowledgeBase, InMemoryKnowledgeBase>();
        services.AddSingleton<IInventoryService, InventoryService>();
        services.AddSingleton<IOrderService, OrderService>();
        services.AddSingleton<ICatalogService, CatalogService>();
        services.AddSingleton<ICustomerService, CustomerService>();
        services.AddSingleton<IFinanceService, FinanceService>();
        services.AddSingleton<INotificationService, NotificationService>();
        services.AddSingleton<IBusinessToolRegistry, BusinessToolRegistry>();
        services.AddSingleton<IGuardrailPolicy, GuardrailPolicy>();
        services.AddSingleton<IAiModel, DeterministicAiModel>();

        return services;
    }
}

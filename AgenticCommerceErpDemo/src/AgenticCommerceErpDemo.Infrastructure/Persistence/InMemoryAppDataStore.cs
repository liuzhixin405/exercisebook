using AgenticCommerceErpDemo.Application.Approvals;
using AgenticCommerceErpDemo.Application.State;
using AgenticCommerceErpDemo.Domain.Catalog;
using AgenticCommerceErpDemo.Domain.Common;
using AgenticCommerceErpDemo.Domain.Customer;
using AgenticCommerceErpDemo.Domain.Inventory;
using AgenticCommerceErpDemo.Domain.Procurement;
using AgenticCommerceErpDemo.Domain.Promotions;

namespace AgenticCommerceErpDemo.Infrastructure.Persistence;

public sealed class InMemoryAppDataStore : IApprovalRepository, IStateReader
{
    public List<Product> Products { get; } = SeedData.Products();
    public List<InventoryItem> Inventory { get; } = SeedData.Inventory();
    public List<SalesForecast> Forecasts { get; } = SeedData.Forecasts();
    public List<CustomerComplaint> Complaints { get; } = SeedData.Complaints();
    public List<PurchaseOrderDraft> PurchaseOrders { get; } = [];
    public List<PromotionDraft> Promotions { get; } = [];
    public List<ApprovalRequest> Approvals { get; } = [];

    public ApprovalRequest? Find(string approvalId)
    {
        return Approvals.FirstOrDefault(x => x.Id == approvalId);
    }

    public IReadOnlyList<ApprovalRequest> ListPending()
    {
        return Approvals.Where(x => x.Status == ApprovalStatus.Pending).ToList();
    }

    public void Add(ApprovalRequest approval)
    {
        Approvals.Add(approval);
    }

    public void MarkDecision(string approvalId, ApprovalStatus status)
    {
        var approval = Find(approvalId) ?? throw new InvalidOperationException("Approval not found.");
        approval.Status = status;
        approval.DecidedAt = DateTimeOffset.UtcNow;
    }

    public object Snapshot() => new
    {
        Products,
        Inventory,
        Forecasts,
        Complaints,
        PurchaseOrders,
        Promotions,
        Approvals
    };
}

using AgenticCommerceErpDemo.Domain.Procurement;

namespace AgenticCommerceErpDemo.Application.Business;

public interface IInventoryService
{
    IReadOnlyList<object> AnalyzeInventoryRisk(string warehouseCode);
    PurchaseOrderDraft CreatePurchaseOrderDraft(string supplierCode, IReadOnlyList<string> skus);
}

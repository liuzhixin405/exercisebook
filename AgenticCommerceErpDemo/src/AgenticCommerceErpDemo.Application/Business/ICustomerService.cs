namespace AgenticCommerceErpDemo.Application.Business;

public interface ICustomerService
{
    IReadOnlyList<object> SummarizeComplaints(IReadOnlyList<string> skus);
}

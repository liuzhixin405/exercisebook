using System.Runtime.InteropServices;

namespace eapi.Service
{
    public interface IOrderService
    {
        Task Create(string sku,int count);
        Task Shipment(int orderId);
        Task Rejected(int orderId);
        Task Completed(int orderId);
        Task CreateTestLock(string sku, int count);
        Task CreateDistLock(string sku, int count);
    }
}

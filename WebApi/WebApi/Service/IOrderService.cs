namespace WebApi.Service
{
    public interface IOrderService
    {
        Task CreateOrder(string sku,int count);
    }
}

namespace WebApi.Service
{
    public interface IProductService
    {
        Task Create(string sku,int count);
    }
}

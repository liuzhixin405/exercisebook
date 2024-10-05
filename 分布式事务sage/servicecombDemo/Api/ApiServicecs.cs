using Repository.Service.Dtos.Orders;
using Repository.Service.Dtos.Products;
using Repository.Service.Orders;
using Repository.Service.Products;

namespace Api
{
    public class ApiServicecs
    {
        private readonly string _orderApiUrl;
        private readonly string _productApiUrl;
        private readonly IHttpClientFactory _httpClientFactory;

        public ApiServicecs(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _orderApiUrl = configuration["ApiUrl:Order"] ?? throw new ArgumentException("ApiUrl:Order is not set in appsettings.json");
            _productApiUrl = configuration["ApiUrl:Product"] ?? throw new ArgumentException("ApiUrl:Product is not set in appsettings.json");
            _httpClientFactory = httpClientFactory;
        }


        public async Task<string> CreateOrderAsync(OrderDto orderDto)
        {
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                var response = await httpClient.PostAsJsonAsync(_orderApiUrl, orderDto);
                response.EnsureSuccessStatusCode();
                return  await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<string> CreateProductAsync(ProductDto productDto)
        {
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                var response = await httpClient.PostAsJsonAsync(_productApiUrl, productDto);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<string> DeleteProductAsync(int productId)
        {
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                var response = await httpClient.DeleteAsync($"{_productApiUrl}/{productId}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<string> DeleteOrderAsync(int orderId)
        {
            using (var httpClient = _httpClientFactory.CreateClient())
            {
                var response = await httpClient.DeleteAsync($"{_orderApiUrl}/{orderId}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}

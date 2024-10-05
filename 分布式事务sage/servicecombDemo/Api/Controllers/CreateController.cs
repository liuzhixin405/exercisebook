using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Text;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreateController : ControllerBase
    {
        private readonly ApiServicecs _apiService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _orderApiUrl;
        private readonly string _productApiUrl;

        public CreateController(ApiServicecs apiServicecs, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _apiService = apiServicecs;
            _httpClientFactory = httpClientFactory;
            _orderApiUrl = configuration["ApiUrl:Order"] ?? throw new ArgumentException("ApiUrl:Order is not set in appsettings.json");
            _productApiUrl = configuration["ApiUrl:Product"] ?? throw new ArgumentException("ApiUrl:Product is not set in appsettings.json");
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateDto createDto)
        {
            int orderId = 0;
            int productId = 0;
            string delOrderStr = string.Empty;
            string delProductStr = string.Empty;
            try
            {
                var orderResult = await _apiService.CreateOrderAsync(new Repository.Service.Dtos.Orders.OrderDto { CustomerName = createDto.CustomerName, TotalAmount = createDto.TotalAmount });
                orderId = int.Parse(orderResult?? "0");
                var productResult = await _apiService.CreateProductAsync(new Repository.Service.Dtos.Products.ProductDto { Name = createDto.Name, Price = createDto.Price });            
                productId = int.Parse(productResult?? "0");
            }
            catch (Exception ex)
            {
                int delOrderCount = 5;
                int delProductCount = 5;
                while (orderId > 0 && delOrderCount > 0)
                {
                    try
                    {
                        var delOrder = await _apiService.DeleteOrderAsync(orderId);
                        var delRow = int.Parse(delOrder); 
                        if (delRow > 0)
                            break;
                    }
                    catch(Exception ex2)
                    {
                        delOrderStr= ex2.Message;  //可以进一步的监控回滚操作成功或失败
                    }
                    finally
                    {
                        delOrderCount--;
                    }
                  
                }
                while (productId > 0 && delProductCount > 0)
                {
                    try
                    {
                        var delProduct = await _apiService.DeleteProductAsync(productId);
                        var delRow2 = int.Parse(delProduct);
                        if (delRow2 > 0)
                            break;
                    }
                    catch(Exception ex3)
                    {
                        delProductStr = ex3.Message;
                    }
                    finally
                    {
                        delProductCount--;
                    }                
                }
            }
            finally
            {
                //记录日志delOrderStr delProductStr 人工干预
            }

            return Ok(orderId > 0 && productId > 0 ? "success" : $"failed:ordermsg:{delOrderStr},productmsg:{delProductStr}");
        }

        public record CreateDto
        {
            public string Name { get; set; }
            public decimal Price { get; set; }
            public decimal TotalAmount { get; set; }
            public string CustomerName { get; set; }
        }
    }
}

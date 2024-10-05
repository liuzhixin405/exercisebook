using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.Service.Dtos.Products;
using Repository.Service.Products;

namespace ProductApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;
        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductDto productDto)
        {
            throw   new Exception("失败测试");
           return Ok( await  _productService.CreateProductAsync(new Product { Name = productDto.Name, Price = productDto.Price }) );

        }
        [HttpDelete("{id}")]
        public async Task<int> Delete(int id)
        {
            return await _productService.DeleteProductAsync(id);
        }
    }
}

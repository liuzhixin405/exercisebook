using AbsEFWork.Implementations.Dto;
using AbsEFWork.Models.Enum;
using BaseEntityFramework.Implementations.Entitys;
using EntityEF.Dto;
using IServiceEF;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AbsEFWork.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UltramanController : ControllerBase
    {
        private readonly IEFCoreService _eFCoreService;
        public UltramanController(IEFCoreService eFCoreService)
        {
            _eFCoreService = eFCoreService;
        }
       
        //分页格式待定
        [HttpGet("Search/{id}")]
        [HttpPut("Update")]  //参数在body  swagger无法做测试
        [HttpPost("Add")]   //参数在body
        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Invoke()
        {
            var request = HttpContext.Request;
            int id = 0;
            var mtype =  (MethodType)Enum.Parse(typeof(MethodType),request.Method,true);
            switch (mtype)
            {
                case MethodType.GET:
                     id = int.Parse(request.RouteValues["id"].ToString());
                    return await Search(id);
                case MethodType.POST:
                case MethodType.PUT:
                    Product product;
                    using (var reader = new StreamReader(HttpContext.Request.Body))
                    {
                        var json = reader.ReadToEnd();
                        product = System.Text.Json.JsonSerializer.Deserialize<Product>(json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        // 处理 myModel 对象...
                    }
                    return await CreateOrUpdate(product);
                    //update or add
                case MethodType.DELETE:
                     id =int.Parse(request.RouteValues["id"].ToString());
                   return await Delete(id);   
                default:
                    throw new Exception("未知");
            }
        }

        

        private async Task<IActionResult> Search(int searchId)
        {
            var result = await _eFCoreService.GetEntity(new GetByIdDto { Id = searchId});
            return Ok(result);
        }

        private async Task<IActionResult> CreateOrUpdate(Product product)
        {
            bool result = false;
            if(product.Id != null)
            {
                result =await _eFCoreService.Update(product);
            }
            else
            {
                result = await _eFCoreService.Add(product);
            }
            return Ok(result);
        }

        private async Task<IActionResult> Delete(int id)
        {
            var result = false;
            var entity = await _eFCoreService.GetEntity(new GetByIdDto { Id=id});
            if (entity != null)
            {
               result =await _eFCoreService.Delete((IRequestDto)entity);
            }  
            return Ok(result);
        }

        private async Task<IActionResult> PageResult()
        {
            return default;
        }
    }
}

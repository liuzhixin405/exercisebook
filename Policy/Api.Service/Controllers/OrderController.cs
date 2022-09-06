using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Api.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        [HttpGet]
        [Route("Query")]
        public async Task<string> Query()
        {
            await Task.CompletedTask;
            throw new Exception("事出反常");
           return JsonConvert.SerializeObject(new {
                Success = true,
                Data = $"ZX{DateTime.Now.ToString("yyyyMMddmmss")}",
                Message = "正常获取数据"
            });
        }
    }
}

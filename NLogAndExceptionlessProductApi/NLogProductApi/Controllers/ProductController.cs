using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace NLogProductApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ILogger _logger;
        public ProductController(ILogger<ProductController> logger)
        {
            _logger = logger;
        }

        [HttpGet("exceptiontest")]
        public string ExceptionTest()
        {
            throw new Exception("发生了未知的异常");
            //try
            //{
            //    throw new Exception("发生了未知的异常");
            //}
            //catch (Exception ex)
            //{
            //    _logger.LogError(ex, $"{HttpContext.Connection.RemoteIpAddress}调用了productapi/product/exceptiontest接口返回了失败");
            //}
            //return "调用失败";
        }

    }
}

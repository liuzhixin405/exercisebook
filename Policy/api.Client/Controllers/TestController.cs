using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Polly;
using StandardLibrary;
using StandardLibrary.HttpHelper;

namespace api.Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        [Route("GetOrder")]
        public async Task<string> GetOrder(string url)
        {
            {
                //string result = string.Empty;
                //var fallbackPolicy = Policy<string>.Handle<Exception>().FallbackAsync("这里是中间商赚差价的那种... ",async (exception, context) => {
                //   await Console.Out.WriteLineAsync("必有妖");
                //    await Console.Out.WriteLineAsync(exception.Exception.Message);
                //});
                //result = await fallbackPolicy.ExecuteAsync(async () => {
                //   await Console.Out.WriteLineAsync("开始调用服务");
                //    return await new RequestApi().InvokeApiAsync(url);
                //});
                //return result;
            }
            //return await new RequestApi().InvokeApiAsync(url);
            return await RequestApiProxy.InvokeApiAsync(url);
        }

        [HttpGet]
        [Route("GetOrderCircuit")]
        public async Task<string> GetOrderCircuit(string url)
        {
            string result = string.Empty;
            var circuitBreakPolicy = Policy.Handle<Exception>().CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: 4,
                durationOfBreak: TimeSpan.FromMilliseconds(60000),
                onBreak: async (exception, breakDelay) => {
                    await Console.Out.WriteLineAsync($" {DateTime.Now}: 已熔断...");
                    await Console.Out.WriteLineAsync($"熔断:{breakDelay.TotalMilliseconds} ms,异常:{exception.Message}");
                },
                onReset: async() => {
                    await Console.Out.WriteLineAsync($"{DateTime.Now}:熔断器关闭...");                  
                },
                onHalfOpen: async () => {
                    await Console.Out.WriteLineAsync($"{DateTime.Now}:熔断时间到,进入半开状态...");
                });
            int i = 50;
            while(i > 0)
            {
                await Console.Out.WriteLineAsync($"第{51-i}此请求....");
                i--;
                try
                {
                    await Task.Delay(2000);
                    result = await circuitBreakPolicy.ExecuteAsync(async () => {
                        await Console.Out.WriteLineAsync($"开始调用服务{DateTime.Now}");
                        return await new RequestApi().InvokeApiAsync(url);
                    });

                }
                catch(Exception ex)
                {
                    result = ex.Message;
                    await Console.Out.WriteLineAsync(result);
                }
                

            }

            return result;
        }
    }
}

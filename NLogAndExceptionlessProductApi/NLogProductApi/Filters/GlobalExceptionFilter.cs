using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace NLogProductApi
{
    public class GlobalExceptionFilter : BaseActionFilterAsync, IAsyncExceptionFilter
    {
        readonly ILogger _logger;
        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            _logger = logger;
        }

        public async Task OnExceptionAsync(ExceptionContext context)
        {
            Exception ex = context.Exception;

           
                _logger.LogError(ex, "");
                //context.Result = Error("系统繁忙", ErrorCodeDefine.ServerError);
                context.Result = Error(ex.Message, 500);
            
            await Task.CompletedTask;
        }
    }
}

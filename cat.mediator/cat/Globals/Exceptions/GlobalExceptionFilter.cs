using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace cat.Globals.Exceptions
{
    public class GlobalExceptionFilter : IAsyncExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;
        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            _logger = logger;
        }
        public Task OnExceptionAsync(ExceptionContext context)
        {
            Exception ex = context.Exception;

            if (ex is CatException catException)
            {
                _logger.LogWarning(catException.Message);
                context.Result = JsonContent(System.Text.Json.JsonSerializer.Serialize(new { 
                
                    Success=false,
                    ErrorCode=-1,
                    Message = catException.Message
                }),200);
            }
            else
            {
                _logger.LogError(ex, "");

                context.Result = JsonContent(System.Text.Json.JsonSerializer.Serialize(new
                {

                    Success = false,
                    ErrorCode = -1,
                    Message = ex.Message
                }),505);
            }
            return Task.CompletedTask;
        }


        /// <summary>
        /// 返回JSON
        /// </summary>
        /// <param name="json">json字符串</param>
        /// <returns></returns>
        public ContentResult JsonContent(string json,int code)
        {
            return new ContentResult { Content = json, StatusCode =code, ContentType = "application/json; charset=utf-8" };
        }
    }
}

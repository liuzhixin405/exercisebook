using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace NLogProductApi
{
    public abstract class BaseActionFilterAsync : Attribute, IAsyncActionFilter
    {
        public async virtual Task OnActionExecuting(ActionExecutingContext context)
        {
            await Task.CompletedTask;
        }

        public async virtual Task OnActionExecuted(ActionExecutedContext context)
        {
            await Task.CompletedTask;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await OnActionExecuting(context);
            if (context.Result == null)
            {
                var nextContext = await next();
                await OnActionExecuted(nextContext);
            }
        }
        /// <summary>
        /// 返回错误
        /// </summary>
        /// <param name="msg">错误提示</param>
        /// <param name="errorCode">错误代码</param>
        /// <returns></returns>
        public ContentResult Error(string msg, int errorCode)
        {
            AjaxResult res = new AjaxResult
            {
                Success = false,
                Msg = msg,
                ErrorCode = errorCode
            };

            return JsonContent(JsonConvert.SerializeObject(res));
        }
        /// <summary>
        /// 返回JSON
        /// </summary>
        /// <param name="json">json字符串</param>
        /// <returns></returns>
        public ContentResult JsonContent(string json)
        {
            return new ContentResult { Content = json, StatusCode = 200, ContentType = "application/json; charset=utf-8" };
        }
        /// <summary>
        /// 返回错误
        /// </summary>
        /// <param name="msg">错误提示</param>
        /// <returns></returns>

        public class AjaxResult
        {
            /// <summary>
            /// 是否成功
            /// </summary>
            public bool Success { get; set; } = true;

            /// <summary>
            /// 错误代码
            /// </summary>
            public int ErrorCode { get; set; } = -1;

            /// <summary>
            /// 返回消息
            /// </summary>
            public string Msg { get; set; }
        }
    }
}

using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AspNetCoreFilters.Filters
{
    public class MySampleActionFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
            Debug.Write(MethodBase.GetCurrentMethod(), context.HttpContext.Request.Path);
        
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            Debug.Write(MethodBase.GetCurrentMethod(), context.HttpContext.Request.Path);
        }
    }
    /// <summary>
    /// 异步过滤器优选
    /// </summary>
    public class SampleAsyncActionFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            Console.WriteLine("执行Action前动作");
            await next();
            Console.WriteLine("执行Action后动作");
        }
    }
}

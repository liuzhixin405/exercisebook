using Microsoft.AspNetCore.Mvc.Filters;

public class AsyncActionFilter : IAsyncActionFilter       //4动作过滤器
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        System.Console.WriteLine($"action执行前:{DateTime.Now.ToString()}");
        await next();
        System.Console.WriteLine($"action执行后:{DateTime.Now.ToString()}");
    }
}

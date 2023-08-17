using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class AsyncResultFilter : IAsyncResultFilter       //5.结果过滤器
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if(context.Result is not EmptyResult){
            await next();
        }else{
            context.Cancel = true;
            System.Console.WriteLine($"result filter 执行完毕:{DateTime.Now.ToString()}");
        }
    }
}

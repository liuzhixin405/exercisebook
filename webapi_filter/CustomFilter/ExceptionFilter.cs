using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class ExceptionFilter : IExceptionFilter    //3异常过滤器
{
    public void OnException(ExceptionContext context)
    {
          System.Console.WriteLine($"异常处理:{nameof(OnException)},{DateTime.Now.ToString()}");
           context.Result= new ContentResult{
            Content =$"已处理异常" 
         };
    }
}

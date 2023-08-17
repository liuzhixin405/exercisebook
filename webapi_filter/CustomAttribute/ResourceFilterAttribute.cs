using Microsoft.AspNetCore.Mvc.Filters;

public class ResourceFilterAttribute : Attribute, IResourceFilter         //2.资源过滤器
{
    public void OnResourceExecuted(ResourceExecutedContext context)
    {
         System.Console.WriteLine($"进入控制器之后执行:{nameof(OnResourceExecuted)},{DateTime.Now.ToString()}");
    }

    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        System.Console.WriteLine($"进入控制器之前执行:{nameof(OnResourceExecuting)},{DateTime.Now.ToString()}");
    }
}

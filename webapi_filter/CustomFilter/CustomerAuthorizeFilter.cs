using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class CustomAuthorizeFilter : IAuthorizationFilter       //1.权限过滤器
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
         System.Console.WriteLine($"进入控制器之前执行:{nameof(OnAuthorization)},{DateTime.Now.ToString()}");
    }
}

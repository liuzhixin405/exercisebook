using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomAuthorizationFilter.Filter
{
    public class AsyncTestAuthorizationFilterAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
                if (context.ActionDescriptor.EndpointMetadata.Any(x => x is AllowAnonymousAttribute))
                {
                    await Task.CompletedTask;
                    return;
                }
                else
                {
                    string sUser = context.HttpContext.Request.Cookies["CurrentUser"];
                    if (sUser == null)
                    {
                        await context.HttpContext.Response.WriteAsync("未登录");
                        context.Result =new RedirectResult("~/Home/Index");    
                        
                    }
                    else
                    {
                        await Task.CompletedTask;
                        return;
                }
                }
        }
    }

    public class TestAuthorizationFilterAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if(context.ActionDescriptor.EndpointMetadata.Any(x=>x is AllowAnonymousAttribute))
            {
                return;
            }
            else
            {
                string sUser = context.HttpContext.Request.Cookies["CurrentUser"];
                if(sUser == null)
                {
                    context.Result = new RedirectResult("~/Home/Index");
                }
                else
                {
                    return;
                }
            }
        }
    }
}

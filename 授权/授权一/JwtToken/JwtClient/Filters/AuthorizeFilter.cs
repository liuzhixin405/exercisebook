using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace JwtClient.Filters
{
    public class AuthorizeFilter : BaseActionFilterAsync, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var request = context.HttpContext.Request;
            
            ////没有 Token 或为空 需要登录
            //if (!request.Headers.TryGetValue("token", out StringValues tokenValue) || string.IsNullOrEmpty(tokenValue))
            //{
            //    context.Result = new ContentResult() { Content=$"error: {JsonConvert.SerializeObject(request.Headers)}"};
            //    return;
            //}
        }
    }
}

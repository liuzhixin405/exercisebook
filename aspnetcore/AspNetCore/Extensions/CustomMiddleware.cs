using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.Extensions
{
    public class CustomMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomMiddleware(RequestDelegate next)
        {
            _next = next;
;        }

        public async Task Invoke(HttpContext httpContext,IMyScopedService svc)
        {
            svc.MyProperty = 1000;
            await _next(httpContext);
        }
    }
}

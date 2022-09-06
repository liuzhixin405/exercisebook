using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ConsoleAppTwo.Extends
{
    public class FoobarSelector : ServiceSelector<IFoobar>, IFoobar
    {
        public FoobarSelector(IHttpContextAccessor httpContextAccessor):base(httpContextAccessor)
        {

        }
        public Task InvokeAsync(HttpContext httpContext) => GetService()?.InvokeAsync(httpContext);
    }
}

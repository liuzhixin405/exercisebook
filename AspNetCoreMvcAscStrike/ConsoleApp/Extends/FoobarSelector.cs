using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ConsoleApp.Extends
{
    public class FoobarSelector : IFoobar
    {
        private static ConcurrentDictionary<Type, string> _source = new ConcurrentDictionary<Type, string>();
        public Task InvokeAsync(HttpContext httpContext)
        {
            return httpContext.RequestServices.GetServices<IFoobar>()
                .FirstOrDefault(x => x != this && GetInvocationSource(x) == httpContext.GetInvocationSource())?.InvokeAsync(httpContext);
        }

        string GetInvocationSource(object service)
        {
            var type = service.GetType();
            return _source.GetOrAdd(type, _ => type.GetCustomAttribute<InvocationSourceAttribute>()?.Source);
        }
    }
}

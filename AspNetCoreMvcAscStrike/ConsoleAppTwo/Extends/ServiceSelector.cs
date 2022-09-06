using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppTwo.Extends
{
    public class ServiceSelector<T> where T:class
    {
        private static ConcurrentDictionary<Type, ServiceFilterAttribute> _fiters = new ConcurrentDictionary<Type, ServiceFilterAttribute>();
        private readonly IHttpContextAccessor _httpContextAccessor;
        protected ServiceSelector(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

        protected T GetService()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            return httpContext.RequestServices.GetServices<T>().FirstOrDefault(x => x != this && GetFilter(x)?.Match(httpContext) == true);
        }
        ServiceFilterAttribute GetFilter(object services)
        {
            var type = services.GetType();
            return _fiters.GetOrAdd(type, _ => type.GetCustomAttribute<ServiceFilterAttribute>());
        }
    }
}

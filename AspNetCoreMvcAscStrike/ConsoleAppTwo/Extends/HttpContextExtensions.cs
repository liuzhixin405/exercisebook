using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleAppTwo.Extends
{
    public static class HttpContextExtensions
    {
        public static string GetInvocationSource(this HttpContext httpContext) => httpContext.Features.Get<IInvocationSourceFeature>()?.Source;
        public static void SetInvocationSource(this HttpContext httpContext, string source) => httpContext.Features.Set<IInvocationSourceFeature>(new InvocationSourceFeature(source));
    }
}

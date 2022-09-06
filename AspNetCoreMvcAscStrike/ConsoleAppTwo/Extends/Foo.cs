using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleAppTwo.Extends
{
    [InvocationSource("App")]
    public class Foo : IFoobar
    {
        public Task InvokeAsync(HttpContext httpContext)
        {
           return httpContext.Response.WriteAsync("Process for App");
        }
    }
    [InvocationSource("MiniApp")]
    public class Bar : IFoobar
    {
        public Task InvokeAsync(HttpContext httpContext)
        {
            return httpContext.Response.WriteAsync("Process for MiniApp");
        }
    }
}

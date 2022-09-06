using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleAppTwo.Extends
{
    public interface IFoobar
    {
        Task InvokeAsync(HttpContext httpContext);
    }
}

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppTwo.Extends
{
    public abstract class ServiceFilterAttribute:Attribute
    {
        public abstract bool Match(HttpContext httpContext);
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class InvocationSourceAttribute : ServiceFilterAttribute
    {
        public string Source { get; }
        public InvocationSourceAttribute(string source)
        {
            Source = source;
        }
        public override bool Match(HttpContext httpContext)
        {
           return httpContext.GetInvocationSource() == Source;
        }
    }
}

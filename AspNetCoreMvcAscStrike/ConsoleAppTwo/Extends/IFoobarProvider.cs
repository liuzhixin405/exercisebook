using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppTwo.Extends
{
    public interface IFoobarProvider
    {
        IFoobar GetService();
    }
    public sealed class FoobarProvider : IFoobarProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public FoobarProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public IFoobar GetService()
        {
            switch (_httpContextAccessor.HttpContext.GetInvocationSource())
            {
                case "App": 
                    return new Foo();
                case "MiniApp": 
                    return new Bar();
                default:
                    return null;
            }
        }
    }
}

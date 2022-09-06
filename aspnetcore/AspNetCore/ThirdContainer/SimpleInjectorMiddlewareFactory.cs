using AspNetCore.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading.Tasks;

namespace AspNetCore.ThirdContainer
{
    

    

    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseSimpleInjectorMiddlewareFactory(this ApplicationBuilder buidler)
        {
           return buidler.UseMiddleware<SimpleInjectorMiddlewareFactory>();
        }
        public static IMiddleware GetInstance(this Container container,Type type)
        {
            return new SimpleInjectorActivatedMiddleware();
        }
    }
    public class SimpleInjectorMiddlewareFactory : IMiddlewareFactory
    {
        private readonly Container _container;
        public SimpleInjectorMiddlewareFactory(Container container)
        {
            _container = container;
        }
        public IMiddleware Create(Type middlewareType)
        {
            return _container.GetInstance(middlewareType) as IMiddleware;
        }

        public void Release(IMiddleware middleware)
        {
            throw new NotImplementedException();
        }
    }

    public class SimpleInjectorActivatedMiddleware : IMiddleware
    {
        private readonly AppDbContext _db;

        public SimpleInjectorActivatedMiddleware() { }
        public SimpleInjectorActivatedMiddleware(AppDbContext db)
        {
            _db = db;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var keyValue = context.Request.Query["key"];
            if (!string.IsNullOrWhiteSpace(keyValue))
            {
                _db.Add(new Request
                {
                    DT= DateTime.UtcNow,
                    MiddlewareActivation = "SimpleInjectorActivatedMiddleware",
                    Value= keyValue
                });

                await _db.SaveChangesAsync(); 
            }
            await next(context);
        }
    }

   
}

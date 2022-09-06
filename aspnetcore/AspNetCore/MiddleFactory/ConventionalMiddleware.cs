using AspNetCore.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.MiddleFactory
{
    public class ConventionalMiddleware
    {

        private readonly RequestDelegate _next;

        public ConventionalMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context,AppDbContext db)
        {
            var keyValue = context.Request.Query["key"];
            if (!string.IsNullOrWhiteSpace(keyValue))
            {
                db.Add(new Request()
                {
                     DT=DateTime.UtcNow,
                     MiddlewareActivation= "FactoryActivatedMiddleware",
                     Value=keyValue
                });
                await db.SaveChangesAsync();
            }
            await _next(context);
        }
    }
}

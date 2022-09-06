
using AspNetCore.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore.MiddleFactory
{
    public class FactoryActivatedMiddleware : IMiddleware
    {
        private readonly AppDbContext _db;
        public FactoryActivatedMiddleware(AppDbContext db)
        {
            _db = db;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var keyValue = context.Request.Query["key"];
            if (!string.IsNullOrWhiteSpace(keyValue))
            {
                _db.Add(new Request()
                {
                    DT = DateTime.UtcNow,
                    MiddlewareActivation = "FactoryActivatedMiddleware",
                    Value = keyValue
                });
                await _db.SaveChangesAsync();
            }
            await next(context);
        }
    }
}

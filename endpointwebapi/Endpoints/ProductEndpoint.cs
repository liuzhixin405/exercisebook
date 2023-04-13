using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Endpoint;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;

namespace webapi.Endpoints
{
    public class ProductEndpoint : IEndpoint<List<Product>, ProductRequestDto, ProductDbContext>
    {
        public void AddRoute(IEndpointRouteBuilder app)
        {
            app.MapPost("api/prodct", async ([FromBody]ProductRequestDto request, ProductDbContext dbContext) =>
            {
               return await  HandleAsync(request, dbContext);
            });
        }
        private static readonly Func<ProductDbContext, string,string,decimal?,DateTime?, IAsyncEnumerable<Product>> GetProductAsync =
            EF.CompileAsyncQuery((ProductDbContext ctx,string name,string description, decimal? price,DateTime? createtime) =>
            ctx.Products.Where(x => string.IsNullOrEmpty(name) || x.Name.Contains(name))
            .Where(x => string.IsNullOrEmpty(description) || x.Description.Contains(description))
            .Where(x => !price.HasValue || price == x.Price)
            .Where(x => !createtime.HasValue || createtime == x.CreateDateTime)
            );
        
            
        public async Task<List<Product>> HandleAsync(ProductRequestDto request,ProductDbContext productDbContext)
        {
            List<Product> list = new List<Product>(); 
            await foreach (var item in GetProductAsync(productDbContext, request.Name,request.Description,request.Price,request.CreateDateTime))
            {
                list.Add(item);
            } ;
            return list;
        }
    }
}

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
    public class ProductEndpoint : IEndpoint<ProductResponseDto<List<Product>>, PageInput<ProductRequestDto>, ProductDbContext>
    {
        public void AddRoute(IEndpointRouteBuilder app)
        {
            app.MapPost("api/prodct", async ([FromBody] PageInput<ProductRequestDto> request, ProductDbContext dbContext) =>
            {
               return await  HandleAsync(request, dbContext);
            });
        }
        private static readonly Func<ProductDbContext,int,int,bool,string, string,string,decimal?,DateTime?, IAsyncEnumerable<Product>> GetProductAsync =
            EF.CompileAsyncQuery((ProductDbContext ctx,int pageIndex,int pageSize,bool sortType,string sortField,string name,string description, decimal? price,DateTime? createtime) =>
            ctx.Products
            .Where(x => string.IsNullOrEmpty(name) || x.Name.Contains(name))
            .Where(x => string.IsNullOrEmpty(description) || x.Description.Contains(description))
            .Where(x => !price.HasValue || price == x.Price)
            .Where(x => !createtime.HasValue || createtime == x.CreateDateTime)
            //.OrderBy(sortField,sortType) //表达式不能翻译TODD:
            .OrderByDescending(x=>x.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
            );

        private static readonly Func<ProductDbContext,  string, string, decimal?, DateTime?, int> GetProductCount =
           EF.CompileQuery((ProductDbContext ctx,  string name, string description, decimal? price, DateTime? createtime) =>
           ctx.Products
           .Where(x => string.IsNullOrEmpty(name) || x.Name.Contains(name))
           .Where(x => string.IsNullOrEmpty(description) || x.Description.Contains(description))
           .Where(x => !price.HasValue || price == x.Price)
           .Where(x => !createtime.HasValue || createtime == x.CreateDateTime).Count()
           );

        public async Task<ProductResponseDto<List<Product>>> HandleAsync(PageInput<ProductRequestDto> request,ProductDbContext productDbContext)
        {
            List<Product> list = new List<Product>(); 
            await foreach (var item in GetProductAsync(productDbContext,request.PageIndex,request.PageRows,request.Desc, request.SortField,request.Search.Name,request.Search.Description,request.Search.Price,request.Search.CreateDateTime))
            {
                list.Add(item);
            } ;
            return new ProductResponseDto<List<Product>> {Data =list ,TotalCount= GetProductCount(productDbContext,  request.Search.Name, request.Search.Description, request.Search.Price, request.Search.CreateDateTime) };
        }

       
    }
}

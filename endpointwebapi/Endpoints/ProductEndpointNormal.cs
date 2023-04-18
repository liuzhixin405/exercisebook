using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Endpoint;
using System.Linq;
using System.Linq.Expressions;

namespace webapi.Endpoints
{
    public class ProductEndpointNormal: IEndpoint<ProductResponseDto<List<Product>>, PageInput<ProductRequestDto>, ProductDbContext>
    {
        public void AddRoute(IEndpointRouteBuilder app)
        {
            app.MapPost("api/productnormal", async ([FromBody] PageInput<ProductRequestDto> request, ProductDbContext dbContext) =>
            {
                return await HandleAsync(request, dbContext);
            });
        }

        public async Task<ProductResponseDto<List<Product>>> HandleAsync(PageInput<ProductRequestDto> request, ProductDbContext dbContext)
        {
            int start = DateTimeOffset.Now.Millisecond;
            var count = 0;
            var query = dbContext.Products.AsQueryable();
            Expression<Func<Product, bool>> exp = x=>true;
            var search = request.Search;
            if (!string.IsNullOrEmpty(request.Search.Name))
            {
                query = query.Where(x=>x.Name.Contains(search.Name));
            }
            if (!string.IsNullOrEmpty(request.Search.Description))
            {
                query = query.Where(x => x.Description.Contains(search.Description));
            }

            if (search.Price.HasValue)
            {
                query = query.Where(x => x.Price == search.Price);
            }

            if(search.CreateDateTime.HasValue)
            {
                query =query.Where(x=>x.CreateDateTime== search.CreateDateTime);
            }
            count = query.Count();
            var result =await query.OrderByDescending(x => x.Id)
             .Skip((request.PageIndex - 1) * request.PageRows)
             .Take(request.PageRows).ToListAsync();
            int end = DateTimeOffset.Now.Millisecond;
            Console.WriteLine($"执行时间为{end-start}ms");
            return new ProductResponseDto<List<Product>> { Data= result,TotalCount=count };
        }
    }
}

using ODataService.Web.Models;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;
using Microsoft.AspNetCore.Builder;
namespace ODataService.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers()
               .AddOData(opt =>
                   opt.AddRouteComponents("odata", ModelBuilder.GetEdmModel()) // 注册 OData 路由
                      .Select()    // 启用 $select
                      .Filter()    // 启用 $filter
                      .OrderBy()   // 启用 $orderby
                      .Expand()    // 启用 $expand
                      .Count()     // 启用 $count
                      
                      .SetMaxTop(10)); // 最大返回记录数限制
            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseStaticFiles();

           app.MapControllers();
          
            app.Run();
        }
    }
}

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
                   opt.AddRouteComponents("odata", ModelBuilder.GetEdmModel()) // ע�� OData ·��
                      .Select()    // ���� $select
                      .Filter()    // ���� $filter
                      .OrderBy()   // ���� $orderby
                      .Expand()    // ���� $expand
                      .Count()     // ���� $count
                      
                      .SetMaxTop(10)); // ��󷵻ؼ�¼������
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

using EFCoreDB.DataLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace EFCoreDB
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<BlogContext_01>(options => options.UseSqlServer("Data Source=PC-202205262203;Initial Catalog=Blog_01;Persist Security Info=False;User ID=sa;Password=1230;MultipleActiveResultSets=true"));
            builder.Services.AddControllers();

            var app =  builder.Build();
            app.MapGet("/", async context =>
            {
                using var scope = context.RequestServices.CreateAsyncScope();
                
                var blogContext = scope.ServiceProvider.GetRequiredService<BlogContext_01>();
                if(!await (blogContext.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).ExistsAsync()) //数据库不存在自动创建，并建表
                {
                    await blogContext.Database.EnsureDeletedAsync();
                    await blogContext.Database.EnsureCreatedAsync();

                }
               await context.Response.WriteAsync("db created");
            });
            app.Run();
        }
    }
}
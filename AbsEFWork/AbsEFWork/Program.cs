
using AbsEFWork.Implementations;
using BaseEntityFramework.Implementations;
using BaseEntityFramework.Implementations.Entitys;
using EntityEF.Models;
using IServiceEF;
using IServiceEF.DefaultImplement;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace BaseEntityFramework
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.AllowSynchronousIO = true;
            });
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
            {
                //options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            })
                ;
            builder.Services.AddDbContext<ProductDbContext>(options => options.UseInMemoryDatabase("memorydb"));
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerDocument();
            builder.Services.AddScoped<IBaseRepository<Product>,DefaultEfCoreRepository<Product, ProductDbContext>>();  //每个实体对用一个repository
           
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork<ProductDbContext>>();   
            builder.Services.AddScoped<IEFCoreService, ProductService>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseOpenApi();
                app.UseSwaggerUi3();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
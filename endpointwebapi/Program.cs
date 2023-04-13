
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Endpoint.Extensions;

namespace webapi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<ProductDbContext>(options =>
            {
                options.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=testdb;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
            });
            builder.Services.AddEndpoints();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            var bContext = app.Services.CreateScope().ServiceProvider.GetService<ProductDbContext>();
            if (!bContext.Products.Any())
            {
                bContext.Products.AddRange(new List<Product> { 
                    new Product
                {
                      Name="铜线",
                      CreateDateTime= DateTime.Now,
                      Description="很细",
                      Price=1.0M
                }, new Product
                {
                      Name="帽子",
                      CreateDateTime= DateTime.Now.AddDays(-3),
                      Description="很漂亮",
                      Price=12.0M
                }, new Product
                {
                      Name="mac笔记本",
                      CreateDateTime= DateTime.Now.AddDays(-6),
                      Description="不便宜",
                      Price=8999.0M
                },
                });
                bContext.SaveChanges();
            }
            app.UseHttpsRedirection();

            app.MapEndpoints();

            app.Run();
        }
    }
}
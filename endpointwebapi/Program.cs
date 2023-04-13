
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Endpoint.Extensions;
using System;
using System.Text;

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
                List<Product> products = new List<Product>();
                string[] pnames = new string[] { "铜线", "帽子", "钢笔", "笔记本", "台灯", "太阳能", "锤子", "钉子" };
                int start = 0x4e00; // 中文字符范围的起始码点
                int end = 0x9fff; // 中文字符范围的结束码点

                for (int i = 0; i < 100; i++)
                {
                    StringBuilder sb = new StringBuilder();
                    int codePoint = Random.Shared.Next(start, end + 1); // 生成一个在中文字符范围内的随机码点
                    char c = (char)codePoint; // 将码点转换为对应的 Unicode 字符
                    sb.Append(c); // 将字符添加到字符串生成器中
                    products.Add(new Product
                    {
                        Name = pnames[Random.Shared.Next(pnames.Length)],
                        CreateDateTime = DateTime.Now.AddDays(Random.Shared.Next(-10, 10)),
                        Description = sb.ToString(),
                        Price = decimal.Parse(Random.Shared.NextDouble().ToString())
                    });
                }
                if(products.Count > 0)
                bContext.Products.AddRange(products);
                bContext.SaveChanges();
            }
            app.UseHttpsRedirection();

            app.MapEndpoints();

            app.Run();
        }
    }
}
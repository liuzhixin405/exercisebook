
using chatgptwriteproject.Context;
using chatgptwriteproject.DbFactories;
using chatgptwriteproject.Models;
using chatgptwriteproject.Repositories;
using chatgptwriteproject.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace chatgptwriteproject
{
    public class Program       //chatgpt只能写基础的逻辑代码
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
                 
            builder.Services.AddDbContextFactory<ApplicationDbContext>(options => options.UseInMemoryDatabase("DefaultConnection"));
            builder.Services.AddScoped<Func<ApplicationDbContext>>(provider => () => provider.GetService<ApplicationDbContext>()??throw new ArgumentNullException("ApplicationDbContext is not inject to program"));
            builder.Services.AddScoped<DbFactory<ApplicationDbContext>>();
            builder.Services.AddScoped(typeof(IProductRepository), typeof(ProductRepository));
            builder.Services.AddTransient<IProductService, ProductService>();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
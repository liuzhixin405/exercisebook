
using chatgptwriteproject.Context;
using chatgptwriteproject.Models;
using chatgptwriteproject.Repositories;
using chatgptwriteproject.Services;
using Microsoft.EntityFrameworkCore;

namespace chatgptwriteproject
{
    public class Program       //chatgpt的基础上改进的
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddScoped(typeof(IRepository<Product>), typeof(ProductRepository));        
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("DefaultConnection"));
            builder.Services.AddScoped(typeof(IUnitOfWork), typeof(ApplicationDbContext));
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
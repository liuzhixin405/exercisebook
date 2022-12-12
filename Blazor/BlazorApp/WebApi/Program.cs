
using Microsoft.EntityFrameworkCore;
using WebApi.Data;
using WebApi.Controllers;

namespace WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddDbContext<CustomerDbContext>(options => options.UseInMemoryDatabase("customer"));
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors(options =>
            {
                options.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins(new[] { "https://localhost:7294" });
            });
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

                      

            app.Run();
        }
    }
}
using MassTransit;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.Configuration;
using Play.Catalog.Service.Entities;
using Play.Common.MassTransit;
using Play.Common.MongoDB;
using Play.Common.Settings;

namespace Play.Catalog.Service
{
    public class Program
    {
        private const string AllowedOriginSetting = "AllowedOrigin";
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            var serviceProvider= builder.Services.BuildServiceProvider();
            var _configuration = serviceProvider.CreateAsyncScope().ServiceProvider.GetService<IConfiguration>();
            var serviceSettings = _configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
            builder.Services.AddMongo().AddMongoRepository<Item>("items").AddMasstransitWithRabbitMq();

            builder.Services.AddControllers(options =>
            {
                options.SuppressAsyncSuffixInActionNames = false;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseCors(builder =>
                {
                    builder.WithOrigins(_configuration[AllowedOriginSetting])
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            }
          
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
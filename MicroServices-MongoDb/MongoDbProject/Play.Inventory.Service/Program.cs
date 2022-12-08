using Microsoft.AspNetCore.OpenApi;
using Play.Common.MassTransit;
using Play.Common.MongoDB;
using Play.Inventory.Service.Clients;
using Play.Inventory.Service.Entities;
using Polly;
using Polly.Timeout;

namespace Play.Inventory.Service
{
    public class Program
    {
        private const string AllowedOriginSetting = "AllowedOrigin";
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var serviceProvider = builder.Services.BuildServiceProvider();
            var _configuration = serviceProvider.CreateAsyncScope().ServiceProvider.GetService<IConfiguration>();
            // Add services to the container.
            builder.Services.AddMongo()
                  .AddMongoRepository<InventoryItem>("inventoryitems")
                  .AddMongoRepository<CatalogItem>("catalogitems")
                  .AddMasstransitWithRabbitMq();

            AddCatalogClient(builder.Services);
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

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

        private static void AddCatalogClient(IServiceCollection services)
        {
            Random jitterer = new Random();

            services.AddHttpClient<CatalogClient>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:5001");
            })
            .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().WaitAndRetryAsync(
                5,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                                + TimeSpan.FromMilliseconds(jitterer.Next(0, 1000)),
                onRetry: (outcome, timespan, retryAttempt) =>
                {
                    var serviceProvider = services.BuildServiceProvider();
                    serviceProvider.GetService<ILogger<CatalogClient>>()?
                        .LogWarning($"Delaying for {timespan.TotalSeconds} seconds, then making retry {retryAttempt}");
                }
            ))
            .AddTransientHttpErrorPolicy(builder => builder.Or<TimeoutRejectedException>().CircuitBreakerAsync(
                3,
                TimeSpan.FromSeconds(15),
                onBreak: (outcome, timespan) =>
                {
                    var serviceProvider = services.BuildServiceProvider();
                    serviceProvider.GetService<ILogger<CatalogClient>>()?
                        .LogWarning($"Opening the circuit for {timespan.TotalSeconds} seconds...");
                },
                onReset: () =>
                {
                    var serviceProvider = services.BuildServiceProvider();
                    serviceProvider.GetService<ILogger<CatalogClient>>()?
                        .LogWarning($"Closing the circuit...");
                }
            ))
            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));
        }
    }
}
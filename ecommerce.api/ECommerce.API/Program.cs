
using ECommerce.API.Application;
using ECommerce.API.Infrastucture;
using ECommerce.API.Models;
using Microsoft.Extensions.Caching.Memory;

namespace ECommerce.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var builder = WebApplication.CreateBuilder(args);

            //// Add services to the container.

            //builder.Services.AddControllers();
            //// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            //builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen();

            //var app = builder.Build();

            //// Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            //    app.UseSwagger();
            //    app.UseSwaggerUI();
            //}

            //app.UseHttpsRedirection();

            //app.UseAuthorization();


            //app.MapControllers();

            //app.Run();

            var builder = WebApplication.CreateBuilder(args);

            // 依赖注入配置
            builder.Services.AddControllers();
            builder.Services.AddMemoryCache();

            // 单例配置
            var config = new AppConfiguration.Builder()
                .WithDatabaseConnection(builder.Configuration["ConnectionString"])
                .WithServiceUrl(builder.Configuration["ServiceUrl"])
                .WithTimeout(30)
                .Build();

            builder.Services.AddSingleton<IAppConfiguration>(config);

            // 策略模式
            builder.Services.AddScoped<CreditCardPayment>();
            builder.Services.AddScoped<PayPalPayment>();
            builder.Services.AddSingleton<IPaymentStrategyFactory, PaymentStrategyFactory>();

            // 装饰器模式 - 链式注册
            builder.Services.AddScoped<IOrderService>(provider =>
            {
                var baseService = new OrderService(
                    provider.GetRequiredService<IOrderRepository>(),
                    provider.GetRequiredService<ILogger<OrderService>>());

                var cachedService = new CachedOrderService(
                    baseService,
                    provider.GetRequiredService<IMemoryCache>(),
                    provider.GetRequiredService<ILogger<CachedOrderService>>());

                return new LoggingOrderService(
                    cachedService,
                    provider.GetRequiredService<ILogger<LoggingOrderService>>());
            });

            // 观察者模式
            builder.Services.AddSingleton<IEventBus, InMemoryEventBus>();
            builder.Services.AddScoped<IEventHandler<OrderCreatedEvent>, OrderCreatedEmailHandler>();
            builder.Services.AddScoped<IEventHandler<OrderCreatedEvent>, OrderCreatedInventoryHandler>();

            // 责任链模式
            builder.Services.AddScoped<IValidationHandler<OrderRequest>>(provider =>
                new OrderValidationHandler(
                    new CustomerValidationHandler()));

            // 仓储
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();

            var app = builder.Build();

            // 配置事件订阅
            var eventBus = app.Services.GetRequiredService<IEventBus>();
            eventBus.Subscribe<OrderCreatedEvent, OrderCreatedEmailHandler>();
            eventBus.Subscribe<OrderCreatedEvent, OrderCreatedInventoryHandler>();

            app.MapControllers();
            app.Run();
        }
    }
}

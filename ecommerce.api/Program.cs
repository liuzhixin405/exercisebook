using ECommerce.API.Application;
using ECommerce.API.Infrastructure;
using ECommerce.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace ECommerce.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 添加注册服务
            builder.Services.AddControllers();
            builder.Services.AddMemoryCache();

            // 配置数据库连接
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ECommerceDbContext>(options =>
                options.UseMySql(connectionString, 
                    ServerVersion.AutoDetect(connectionString))
            );

            // 配置应用设置
            var config = new AppConfiguration.Builder()
                .WithDatabaseConnection(connectionString)
                .WithServiceUrl(builder.Configuration["AppSettings:ServiceUrl"])
                .WithTimeout(builder.Configuration.GetValue<int>("AppSettings:TimeoutSeconds"))
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
            builder.Services.AddScoped<IDbInitializer, DbInitializer>();
            
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            
            var app = builder.Build();

            // 初始化数据库
            using (var scope = app.Services.CreateScope())
            {
                var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
                try
                {
                    await dbInitializer.InitializeAsync();
                }
                catch (Exception ex)
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while initializing the database.");
                    throw;
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // 订阅事件处理
            var eventBus = app.Services.GetRequiredService<IEventBus>();
            eventBus.Subscribe<OrderCreatedEvent, OrderCreatedEmailHandler>();
            eventBus.Subscribe<OrderCreatedEvent, OrderCreatedInventoryHandler>();

            app.MapControllers();
            app.Run();
        }
    }
}

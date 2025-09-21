using ECommerce.Application.Services;
using ECommerce.Application.EventHandlers;
using ECommerce.Domain.Interfaces;
using ECommerce.Infrastructure.Data;
using ECommerce.Infrastructure.Repositories;
using ECommerce.Infrastructure.Services;
using ECommerce.Infrastructure.EventBus;
using Microsoft.EntityFrameworkCore;
using ECommerce.API.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ECommerce.API.BackgroundServices;
using ECommerce.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ECommerce.API;
using RabbitMQ.Client;

// 检查是否是数据库初始化命令
if (args.Length > 0 && args[0].StartsWith("db-"))
{
    await RunDatabaseInitTool(args);
    return;
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var secretKey = builder.Configuration["JwtSettings:SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:3001")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
//#if DEBUG
//builder.Services.AddDbContext<ECommerceDbContext>(options =>
  //  options.UseInMemoryDatabase("ECommerceTestDb"));
//#else
//Add DbContext
builder.Services.AddDbContext<ECommerceDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
       ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
    ));
//#endif


// Add Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IAddressRepository, AddressRepository>();
builder.Services.AddScoped<IInventoryTransactionRepository, InventoryTransactionRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
// Add Core Business Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IPaymentService, DefaultPaymentService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();

// Add Infrastructure Services
builder.Services.AddScoped<IEmailService, DefaultEmailService>();
builder.Services.AddScoped<ICacheService, DefaultCacheService>();
builder.Services.AddScoped<INotificationService, DefaultNotificationService>();
builder.Services.AddScoped<IStatisticsService, DefaultStatisticsService>();
builder.Services.AddScoped<IJwtService, JwtService>();

// Add Event Bus Services
builder.Services.AddRabbitMQEventBus(builder.Configuration);

// Add Event Handlers (简化版本，只依赖Logger)
builder.Services.AddScoped<OrderCreatedEventHandler>();
builder.Services.AddScoped<OrderPaidEventHandler>();
builder.Services.AddScoped<OrderStatusChangedEventHandler>();
builder.Services.AddScoped<OrderCancelledEventHandler>();
builder.Services.AddScoped<InventoryUpdatedEventHandler>();
builder.Services.AddScoped<PaymentProcessedEventHandler>();
builder.Services.AddScoped<UserRegisteredEventHandler>();
builder.Services.AddScoped<PaymentSucceededEventHandler>();
builder.Services.AddScoped<PaymentFailedEventHandler>();
builder.Services.AddScoped<StockLockedEventHandler>();

// 添加后台服务 - 使用条件注册，避免RabbitMQ不可用时阻止应用启动
if (builder.Environment.IsDevelopment() || IsRabbitMQAvailable())
{
    builder.Services.AddHostedService<EventBusStartupService>();
    builder.Services.AddHostedService<OrderExpirationConsumer>();
    builder.Services.AddHostedService<OrderConfirmationConsumer>();
    builder.Services.AddHostedService<OrderShipmentConsumer>();
    builder.Services.AddHostedService<OrderCompletionConsumer>();
}
else
{
    builder.Services.AddLogging(logging =>
    {
        logging.AddConsole();
    });
    // 在非开发环境或RabbitMQ不可用时，记录警告但不阻止启动
    Console.WriteLine("Warning: RabbitMQ services are disabled due to connection issues.");
}
builder.Services.AddSingleton<IRabbitMqDelayPublisher, RabbitMqDelayPublisher>();
builder.Services.AddSingleton<IRabbitMqConnectionProvider, RabbitMqConnectionProvider>();
builder.Services.AddSingleton<ECommerce.Domain.Interfaces.IOrderMessagePublisher, OrderMessagePublisher>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 配置静态文件服务
app.UseStaticFiles();

// 添加CORS中间件
app.UseCors("AllowFrontend");

// 添加全局异常处理中间件
app.UseGlobalExceptionMiddleware();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

/// <summary>
/// 检查RabbitMQ是否可用
/// </summary>
static bool IsRabbitMQAvailable()
{
    try
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest",
            RequestedConnectionTimeout = TimeSpan.FromSeconds(5)
        };
        using var connection = factory.CreateConnection();
        return connection.IsOpen;
    }
    catch
    {
        return false;
    }
}

/// <summary>
/// 运行数据库初始化工具
/// </summary>
static async Task RunDatabaseInitTool(string[] args)
{
    // 构建配置
    var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false)
        .AddJsonFile("appsettings.Development.json", optional: true)
        .Build();

    // 构建日志
    using var loggerFactory = LoggerFactory.Create(builder =>
        builder.AddConsole().SetMinimumLevel(LogLevel.Information));

    var logger = loggerFactory.CreateLogger<DatabaseInitTool>();

    // 创建工具实例并执行
    var tool = new DatabaseInitTool(configuration, logger);
    
    // 移除 "db-" 前缀
    var cleanArgs = args.Select(arg => arg.StartsWith("db-") ? arg.Substring(3) : arg).ToArray();
    
    await tool.InitializeDatabaseAsync(cleanArgs);
}
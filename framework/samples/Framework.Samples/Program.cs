using Framework.Extensions;
using Framework.Samples.Commands;
using Framework.Samples.Events;
using Framework.Samples.Middleware;
using Framework.Samples.Services;
using Framework.Samples.States;
using Framework.Samples.Strategies;

var builder = WebApplication.CreateBuilder(args);

// 添加框架服务
builder.Services.AddFramework(framework =>
{
    // 配置服务
    framework.ConfigureServices(container =>
    {
        container.RegisterSingleton<IUserService, UserService>();
        container.RegisterSingleton<IEmailService, EmailService>();
    });

    // 配置中间件
    framework.ConfigureMiddleware(pipeline =>
    {
        pipeline.Use<LoggingMiddleware>();
        pipeline.Use<AuthenticationMiddleware>();
        pipeline.Use<AuthorizationMiddleware>();
    });
});

// 添加示例服务
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// 添加中间件
builder.Services.AddMiddleware<LoggingMiddleware>();
builder.Services.AddMiddleware<AuthenticationMiddleware>();
builder.Services.AddMiddleware<AuthorizationMiddleware>();

// 添加事件处理器
builder.Services.AddEventHandler<UserCreatedEvent, UserCreatedEventHandler>();
builder.Services.AddEventHandler<UserUpdatedEvent, UserUpdatedEventHandler>();

// 添加命令处理器
builder.Services.AddCommandHandler<CreateUserCommand, CreateUserCommandHandler>();
builder.Services.AddCommandHandler<UpdateUserCommand, UpdateUserCommandHandler>();

// 添加策略
builder.Services.AddStrategy<EmailValidationStrategy>();
builder.Services.AddStrategy<PasswordValidationStrategy>();

// 添加状态
builder.Services.AddState<UserRegistrationState>();
builder.Services.AddState<UserActiveState>();

// 添加访问者
builder.Services.AddVisitor<User, UserAuditVisitor>();

// 添加拦截器
builder.Services.AddInterceptor<LoggingInterceptor>();
builder.Services.AddInterceptor<CachingInterceptor>();

// 添加控制器
builder.Services.AddControllers();

// 添加Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 配置HTTP请求管道
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// 使用框架
app.UseFramework();

app.MapControllers();

// 演示框架功能
await DemonstrateFrameworkFeatures(app);

app.Run();

/// <summary>
/// 演示框架功能
/// </summary>
static async Task DemonstrateFrameworkFeatures(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var framework = scope.ServiceProvider.GetRequiredService<Framework.Core.Abstractions.IApplicationFramework>();

    // 演示事件系统
    Console.WriteLine("=== 演示事件系统 ===");
    var userCreatedEvent = new UserCreatedEvent { UserId = Guid.NewGuid(), UserName = "张三", Email = "zhangsan@example.com" };
    await framework.EventBus.PublishAsync(userCreatedEvent);

    // 演示命令系统
    Console.WriteLine("\n=== 演示命令系统 ===");
    var createUserCommand = new CreateUserCommand { UserName = "李四", Email = "lisi@example.com", Password = "password123" };
    await framework.CommandBus.SendAsync(createUserCommand);

    // 演示策略系统
    Console.WriteLine("\n=== 演示策略系统 ===");
    var emailStrategy = framework.StrategyContext.GetStrategy<EmailValidationStrategy>();
    if (emailStrategy != null)
    {
        var isValid = await emailStrategy.ExecuteAsync("test@example.com");
        Console.WriteLine($"邮箱验证结果: {isValid}");
    }

    // 演示状态管理
    Console.WriteLine("\n=== 演示状态管理 ===");
    var registrationState = new UserRegistrationState();
    await framework.StateManager.SetState(registrationState);
    Console.WriteLine($"当前状态: {framework.StateManager.GetCurrentState<UserRegistrationState>()?.Name}");

    // 演示访问者模式
    Console.WriteLine("\n=== 演示访问者模式 ===");
    var user = new User { Id = Guid.NewGuid(), Name = "王五", Email = "wangwu@example.com" };
    await framework.VisitorRegistry.VisitAsync(user);

    Console.WriteLine("\n=== 框架功能演示完成 ===");
}

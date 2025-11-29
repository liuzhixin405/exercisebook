using Framework.Extensions;
using Framework.Samples.Commands;
using Framework.Samples.Events;
using Framework.Samples.Middleware;
using Framework.Samples.Services;
using Framework.Samples.States;
using Framework.Samples.Strategies;
using Framework.Samples.Visitors;
using Framework.Samples.Interceptors;
using Framework.Samples.Decorators;
using Framework.Samples.Iterators;
using Framework.Samples.Mediators;
using Framework.Samples.Mementos;
using Framework.Samples.Templates;
using Framework.Samples.Singletons;
using Framework.Samples.Factories;
using Framework.Samples.Builders;
using Framework.Infrastructure.Memento;
using Framework.Infrastructure.Mediators;
using Framework.Infrastructure.Decorators;

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

// Start the host so hosted services (like CommandHandlerRegistrar) run and register handlers
await app.StartAsync();

// 演示框架功能
await DemonstrateFrameworkFeatures(app);

// 等待应用停止（不要再次调用 Start/Run，避免重复启动）
await app.WaitForShutdownAsync();

/// <summary>
/// 演示框架功能
/// </summary>
static async Task DemonstrateFrameworkFeatures(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var framework = scope.ServiceProvider.GetRequiredService<Framework.Core.Abstractions.IApplicationFramework>();

    Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
    Console.WriteLine("║           Framework 设计模式完整示例演示                      ║");
    Console.WriteLine("╚══════════════════════════════════════════════════════════════╝\n");

    // 1. 创建型模式
    await DemonstrateCreationalPatterns();

    // 2. 结构型模式
    await DemonstrateStructuralPatterns(framework);

    // 3. 行为型模式
    await DemonstrateBehavioralPatterns(framework);

    // 4. 原有框架功能
    await DemonstrateFrameworkCore(framework);

    Console.WriteLine("\n╔══════════════════════════════════════════════════════════════╗");
    Console.WriteLine("║                   所有演示完成！                              ║");
    Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
}

/// <summary>
/// 演示创建型模式
/// </summary>
static async Task DemonstrateCreationalPatterns()
{
    Console.WriteLine("┌──────────────────────────────────────────────────────────────┐");
    Console.WriteLine("│                    创建型设计模式                              │");
    Console.WriteLine("└──────────────────────────────────────────────────────────────┘\n");

    // 1. 单例模式
    Console.WriteLine("━━━ 1. 单例模式 (Singleton Pattern) ━━━");
    var config = Framework.Samples.Singletons.ConfigurationManager.Instance;
    config.SetSetting("Theme", "Dark");
    Console.WriteLine($"配置值: {config.GetSetting("Theme")}");

    var logger = LogManager.Instance;
    logger.Log("应用程序启动");

    var dbPool = DatabaseConnectionPool.Instance;
    var conn = dbPool.GetConnection();
    conn.Execute("SELECT * FROM Users");
    dbPool.ReleaseConnection(conn);
    Console.WriteLine($"连接池状态: {dbPool.AvailableConnections}/{dbPool.TotalConnections} 可用\n");

    // 2. 工厂模式
    Console.WriteLine("━━━ 2. 简单工厂模式 (Simple Factory Pattern) ━━━");
    var emailNotification = NotificationFactory.CreateNotification(NotificationChannel.Email);
    await emailNotification.SendAsync("user@example.com", "欢迎注册！");

    var smsNotification = NotificationFactory.CreateNotification(NotificationChannel.SMS);
    await smsNotification.SendAsync("13800138000", "验证码: 123456\n");

    Console.WriteLine("━━━ 3. 工厂方法模式 (Factory Method Pattern) ━━━");
    var fileLoggerFactory = new FileLoggerFactory();
    fileLoggerFactory.LogMessage("文件日志测试");

    var cloudLoggerFactory = new CloudLoggerFactory();
    cloudLoggerFactory.LogMessage("云日志测试\n");

    Console.WriteLine("━━━ 4. 抽象工厂模式 (Abstract Factory Pattern) ━━━");
    IUIFactory windowsFactory = new WindowsUIFactory();
    var winButton = windowsFactory.CreateButton();
    var winTextBox = windowsFactory.CreateTextBox();
    winButton.Render();
    winTextBox.Render();

    IUIFactory macFactory = new MacUIFactory();
    var macButton = macFactory.CreateButton();
    var macTextBox = macFactory.CreateTextBox();
    macButton.Render();
    macTextBox.Render();
    Console.WriteLine();

    // 3. 建造者模式
    Console.WriteLine("━━━ 5. 建造者模式 (Builder Pattern) ━━━");
    var computerDirector = new ComputerDirector();
    var builder = new ComputerBuilder();
    var gamingPC = computerDirector.BuildGamingComputer(builder);
    gamingPC.Display();

    Console.WriteLine();
    var httpRequest = HttpRequestBuilder.Post("https://api.example.com/users")
        .AddHeader("Content-Type", "application/json")
        .AddHeader("Authorization", "Bearer token123")
        .AddQueryParameter("version", "v1")
        .SetBody("{\"name\":\"张三\"}")
        .SetTimeout(60)
        .Build();
    httpRequest.Display();
    Console.WriteLine();

    await Task.CompletedTask;
}

/// <summary>
/// 演示结构型模式
/// </summary>
static async Task DemonstrateStructuralPatterns(Framework.Core.Abstractions.IApplicationFramework framework)
{
    Console.WriteLine("┌──────────────────────────────────────────────────────────────┐");
    Console.WriteLine("│                    结构型设计模式                              │");
    Console.WriteLine("└──────────────────────────────────────────────────────────────┘\n");

    // 1. 装饰器模式
    Console.WriteLine("━━━ 6. 装饰器模式 (Decorator Pattern) ━━━");
    var cacheProvider = new CacheProvider();
    cacheProvider.Set("user:123", "张三", TimeSpan.FromMinutes(5));
    var cachedValue = cacheProvider.Get<string>("user:123");
    Console.WriteLine($"从缓存获取: {cachedValue}");

    var perfMonitor = new PerformanceMonitor();
    using (perfMonitor.StartMonitoring("数据库查询"))
    {
        await Task.Delay(50); // 模拟操作
    }

    var exceptionHandler = new ExceptionHandler();
    await exceptionHandler.HandleExceptionAsync(new InvalidOperationException("测试异常"));

    var auditLogger = new AuditLogger();
    await auditLogger.LogAuditAsync("用户登录", new { UserId = 123, IP = "192.168.1.1" });
    Console.WriteLine();

    // 2. 代理模式（已在框架中）
    Console.WriteLine("━━━ 7. 代理模式 (Proxy Pattern) ━━━");
    Console.WriteLine("代理模式已在框架的 ProxyFactory 中实现");
    Console.WriteLine("用于动态创建服务代理和拦截器\n");

    // 3. 外观模式
    Console.WriteLine("━━━ 8. 外观模式 (Facade Pattern) ━━━");
    Console.WriteLine("外观模式已在 ApplicationFramework 中实现");
    Console.WriteLine("为复杂的子系统（EventBus, CommandBus等）提供统一接口\n");
}

/// <summary>
/// 演示行为型模式
/// </summary>
static async Task DemonstrateBehavioralPatterns(Framework.Core.Abstractions.IApplicationFramework framework)
{
    Console.WriteLine("┌──────────────────────────────────────────────────────────────┐");
    Console.WriteLine("│                    行为型设计模式                              │");
    Console.WriteLine("└──────────────────────────────────────────────────────────────┘\n");

    // 1. 策略模式（已在框架中）
    Console.WriteLine("━━━ 9. 策略模式 (Strategy Pattern) ━━━");
    var emailStrategy = framework.StrategyContext.GetStrategy<EmailValidationStrategy>();
    if (emailStrategy != null)
    {
        var isValid = await emailStrategy.ExecuteAsync("test@example.com");
        Console.WriteLine($"邮箱验证结果: {isValid}");
    }
    Console.WriteLine();

    // 2. 观察者模式（事件系统）
    Console.WriteLine("━━━ 10. 观察者模式 (Observer Pattern) ━━━");
    Console.WriteLine("观察者模式已在 EventBus 中实现");
    var userCreatedEvent = new UserCreatedEvent 
    { 
        UserId = Guid.NewGuid(), 
        UserName = "张三", 
        Email = "zhangsan@example.com" 
    };
    await framework.EventBus.PublishAsync(userCreatedEvent);
    Console.WriteLine();

    // 3. 命令模式
    Console.WriteLine("━━━ 11. 命令模式 (Command Pattern) ━━━");
    var createUserCommand = new CreateUserCommand 
    { 
        UserName = "李四", 
        Email = "lisi@example.com", 
        Password = "password123" 
    };
    await framework.CommandBus.SendAsync(createUserCommand);
    Console.WriteLine();

    // 4. 状态模式
    Console.WriteLine("━━━ 12. 状态模式 (State Pattern) ━━━");
    var registrationState = new UserRegistrationState();
    framework.StateManager.SetState(registrationState);
    Console.WriteLine($"当前状态: {framework.StateManager.GetCurrentState<UserRegistrationState>()?.Name}");
    
    var activeState = new UserActiveState();
    framework.StateManager.TransitionToAsync(activeState);
    Console.WriteLine($"状态转换后: {framework.StateManager.GetCurrentState<UserActiveState>()?.Name}\n");

    // 5. 访问者模式
    Console.WriteLine("━━━ 13. 访问者模式 (Visitor Pattern) ━━━");
    var user = new User { Id = Guid.NewGuid(), Name = "王五", Email = "wangwu@example.com" };
    await framework.VisitorRegistry.VisitAsync(user);
    Console.WriteLine();

    // 6. 迭代器模式
    Console.WriteLine("━━━ 14. 迭代器模式 (Iterator Pattern) ━━━");
    var userCollection = new UserCollection();
    userCollection.AddUser(new UserData { Id = Guid.NewGuid(), Name = "用户1", Email = "user1@test.com", Role = "Admin" });
    userCollection.AddUser(new UserData { Id = Guid.NewGuid(), Name = "用户2", Email = "user2@test.com", Role = "User" });
    userCollection.AddUser(new UserData { Id = Guid.NewGuid(), Name = "用户3", Email = "user3@test.com", Role = "Admin" });

    var iterator = userCollection.GetIterator();
    Console.WriteLine("[迭代器示例] 遍历所有用户:");
    while (iterator.MoveNext())
    {
        var userData = iterator.Current;
        Console.WriteLine($"  - {userData.Name} ({userData.Role})");
    }

    // 使用过滤迭代器
    iterator.Reset();
    var adminIterator = new FilterIterator<UserData>(iterator, u => u.Role == "Admin");
    Console.WriteLine("[迭代器示例] 仅遍历管理员:");
    while (adminIterator.MoveNext())
    {
        var userData = adminIterator.Current;
        Console.WriteLine($"  - {userData.Name} ({userData.Role})");
    }
    Console.WriteLine();

    // 7. 中介者模式
    Console.WriteLine("━━━ 15. 中介者模式 (Mediator Pattern) ━━━");
    var mediator = new Mediator();
    mediator.RegisterHandler(new OrderMessageHandler());
    mediator.RegisterHandler(new PaymentMessageHandler());
    mediator.RegisterHandler(new NotificationMessageHandler());

    var orderMsg = new OrderMessage 
    { 
        OrderId = Guid.NewGuid(), 
        ProductName = "笔记本电脑", 
        Quantity = 1, 
        Price = 5999 
    };
    await mediator.SendAsync(orderMsg);

    var paymentMsg = new PaymentMessage 
    { 
        PaymentId = Guid.NewGuid(), 
        OrderId = orderMsg.OrderId, 
        Amount = 5999, 
        PaymentMethod = "支付宝" 
    };
    var paymentResult = await mediator.SendAsync<PaymentMessage, PaymentResult>(paymentMsg);
    Console.WriteLine($"[中介者示例] 支付结果: {paymentResult.Message}");

    var notificationMsg = new NotificationMessage 
    { 
        Recipient = "user@example.com", 
        Subject = "订单确认", 
        Content = "您的订单已确认", 
        Type = NotificationType.Email 
    };
    await mediator.SendAsync(notificationMsg);
    Console.WriteLine();

    // 8. 备忘录模式
    Console.WriteLine("━━━ 16. 备忘录模式 (Memento Pattern) ━━━");
    var mementoManager = new MementoManager();
    var docEditor = new DocumentEditor(mementoManager);

    docEditor.Title = "我的文档";
    docEditor.Content = "这是第一版内容";
    var save1 = docEditor.Save();

    docEditor.Content = "这是第二版内容，添加了更多信息";
    var save2 = docEditor.Save();

    docEditor.Content = "这是第三版内容，进行了大量修改";
    docEditor.Save();

    Console.WriteLine($"[备忘录示例] 当前内容长度: {docEditor.Content.Length}");
    docEditor.Restore(save1);
    Console.WriteLine($"[备忘录示例] 恢复到第一版后内容: {docEditor.Content}");

    // 游戏存档示例
    var gameManager = new GameSaveManager(new MementoManager());
    gameManager.Play();
    gameManager.LevelUp();
    var gameSave1 = gameManager.SaveGame();
    
    gameManager.Play();
    gameManager.Play();
    gameManager.LevelUp();
    
    gameManager.LoadGame(gameSave1);
    gameManager.DisplayCurrentState();
    Console.WriteLine();

    // 9. 模板方法模式
    Console.WriteLine("━━━ 17. 模板方法模式 (Template Method Pattern) ━━━");
    var registrationTemplate = new UserRegistrationTemplate();
    var regContext = new UserRegistrationContext
    {
        UserName = "测试用户",
        Email = "test@example.com",
        Password = "pass123456"
    };
    var regResult = await registrationTemplate.ExecuteAsync(regContext);
    Console.WriteLine($"[模板方法示例] 注册结果: {regResult.Message}");

    Console.WriteLine();
    var orderTemplate = new OrderProcessingTemplate();
    var orderContext = new OrderContext
    {
        CustomerName = "客户A",
        Items = new List<OrderItem>
        {
            new OrderItem { ProductName = "商品1", Quantity = 2, Price = 99.99m },
            new OrderItem { ProductName = "商品2", Quantity = 1, Price = 199.99m }
        },
        TotalAmount = 399.97m
    };
    var orderResult = await orderTemplate.ExecuteAsync(orderContext);
    Console.WriteLine($"[模板方法示例] 订单结果: {orderResult.Message}\n");
}

/// <summary>
/// 演示框架核心功能
/// </summary>
static async Task DemonstrateFrameworkCore(Framework.Core.Abstractions.IApplicationFramework framework)
{
    Console.WriteLine("┌──────────────────────────────────────────────────────────────┐");
    Console.WriteLine("│                    框架核心功能                                │");
    Console.WriteLine("└──────────────────────────────────────────────────────────────┘\n");

    Console.WriteLine("━━━ 中间件管道 (Middleware Pipeline) ━━━");
    Console.WriteLine("中间件系统使用了责任链模式");
    Console.WriteLine("已注册: LoggingMiddleware, AuthenticationMiddleware, AuthorizationMiddleware\n");

    Console.WriteLine("━━━ 依赖注入容器 (Service Container) ━━━");
    Console.WriteLine("容器使用了工厂模式和单例模式");
    Console.WriteLine("提供服务注册、解析和生命周期管理\n");

    Console.WriteLine("━━━ 拦截器 (Interceptor) ━━━");
    Console.WriteLine("拦截器使用了装饰器模式和代理模式");
    Console.WriteLine("已注册: LoggingInterceptor, CachingInterceptor\n");

    await Task.CompletedTask;
}

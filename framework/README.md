这是一个ASP.NET Core框架，旨在提供高度抽象、解耦和灵活扩展的架构。

## 🎯 设计目标

- **高度抽象**: 通过接口和抽象类提供清晰的抽象层
- **松耦合**: 使用依赖注入和事件驱动架构实现组件间的松耦合
- **灵活扩展**: 支持插件化架构和模块化设计

## 🏗️ 架构设计

### 核心组件

```
Framework/
├── Core/                          # 核心抽象层
│   ├── Abstractions/             # 接口定义
│   ├── Exceptions/               # 异常定义
│   └── Constants/                # 常量定义
├── Infrastructure/               # 基础设施层
│   ├── Configuration/            # 配置系统
│   ├── Container/                # 容器系统
│   ├── Middleware/               # 中间件系统
│   ├── Events/                   # 事件系统
│   ├── Commands/                 # 命令系统
│   ├── States/                   # 状态管理
│   ├── Strategies/               # 策略系统
│   ├── Proxies/                  # 代理系统
│   ├── Visitors/                 # 访问者系统
│   └── Extensions/               # 扩展方法
├── Patterns/                     # 设计模式实现
│   ├── Creational/               # 创建型模式
│   ├── Structural/               # 结构型模式
│   └── Behavioral/               # 行为型模式
├── Extensions/                   # 框架扩展
└── Samples/                      # 示例项目
```

## 🎨 设计模式实现

### 创建型模式 (Creational Patterns)

1. **单例模式 (Singleton)**

   - `ServiceContainer`: 服务容器单例
   - `StateManager`: 状态管理器单例
2. **工厂模式 (Factory)**

   - `ServiceFactory<T>`: 服务工厂
   - `ProxyFactory`: 代理工厂
3. **建造者模式 (Builder)**

   - `ConfigurationBuilder`: 配置构建器
   - `ApplicationFramework`: 框架构建器
4. **原型模式 (Prototype)**

   - `Memento`: 状态备忘录

### 结构型模式 (Structural Patterns)

5. **适配器模式 (Adapter)**

   - `ConfigurationAdapter`: 配置适配器
   - `EnvironmentConfigurationAdapter`: 环境变量适配器
6. **装饰器模式 (Decorator)**

   - `AuditingEventBus`: 审计事件总线装饰器
   - `PerformanceMonitoringMiddleware`: 性能监控中间件装饰器
7. **外观模式 (Facade)**

   - `ApplicationFramework`: 框架主入口外观
8. **代理模式 (Proxy)**

   - `ProxyFactory`: 代理工厂
   - `ReflectionProxy<T>`: 反射代理
9. **桥接模式 (Bridge)**

   - 抽象与实现分离的架构设计

### 行为型模式 (Behavioral Patterns)

10. **策略模式 (Strategy)**

    - `StrategyContext`: 策略上下文
    - `EmailValidationStrategy`: 邮箱验证策略
11. **观察者模式 (Observer)**

    - `EventBus`: 事件总线
    - `IEventHandler<T>`: 事件处理器
12. **命令模式 (Command)**

    - `CommandBus`: 命令总线
    - `ICommandHandler<T>`: 命令处理器
13. **责任链模式 (Chain of Responsibility)**

    - `MiddlewarePipeline`: 中间件管道
14. **模板方法模式 (Template Method)**

    - `TemplateMethodBase<TContext, TResult>`: 模板方法基类
15. **状态模式 (State)**

    - `StateManager`: 状态管理器
    - `IState`: 状态接口
16. **中介者模式 (Mediator)**

    - `Mediator`: 中介者
    - `IMessageHandler<T>`: 消息处理器
17. **访问者模式 (Visitor)**

    - `VisitorRegistry`: 访问者注册器
    - `IVisitor<T>`: 访问者接口
18. **迭代器模式 (Iterator)**

    - `Iterator<T>`: 迭代器
    - `IAggregate<T>`: 聚合接口
19. **备忘录模式 (Memento)**

    - `MementoManager`: 备忘录管理器
    - `Memento<T>`: 备忘录

## 🚀 快速开始

### 1. 安装依赖

```bash
dotnet restore
```

### 2. 配置服务

```csharp
var builder = WebApplication.CreateBuilder(args);

// 添加框架服务
builder.Services.AddFramework(framework =>
{
    // 配置服务
    framework.ConfigureServices(container =>
    {
        container.RegisterSingleton<IUserService, UserService>();
    });

    // 配置中间件
    framework.ConfigureMiddleware(pipeline =>
    {
        pipeline.Use<LoggingMiddleware>();
        pipeline.Use<AuthenticationMiddleware>();
    });
});
```

### 3. 使用框架

```csharp
var app = builder.Build();

// 使用框架中间件
app.UseFramework();

app.Run();
```

## 📝 使用示例

### 事件系统

```csharp
// 发布事件
var userCreatedEvent = new UserCreatedEvent 
{ 
    UserId = Guid.NewGuid(), 
    UserName = "张三", 
    Email = "zhangsan@example.com" 
};
await eventBus.PublishAsync(userCreatedEvent);

// 订阅事件
eventBus.Subscribe<UserCreatedEvent>(async @event =>
{
    Console.WriteLine($"用户创建: {@event.UserName}");
});
```

### 命令系统

```csharp
// 发送命令
var createUserCommand = new CreateUserCommand 
{ 
    UserName = "李四", 
    Email = "lisi@example.com", 
    Password = "password123" 
};
await commandBus.SendAsync(createUserCommand);
```

### 策略系统

```csharp
// 执行策略
var emailStrategy = strategyContext.GetStrategy<EmailValidationStrategy>();
var isValid = await emailStrategy.ExecuteAsync("test@example.com");
```

### 状态管理

```csharp
// 设置状态
var registrationState = new UserRegistrationState();
await stateManager.SetState(registrationState);

// 状态转换
var activeState = new UserActiveState();
await stateManager.TransitionToAsync(activeState);
```

### 中间件管道

```csharp
// 配置中间件
middlewarePipeline
    .Use<LoggingMiddleware>()
    .Use<AuthenticationMiddleware>()
    .Use<AuthorizationMiddleware>();
```

## 🔧 扩展框架

### 添加自定义中间件

```csharp
public class CustomMiddleware : IMiddleware
{
    public string Name => "CustomMiddleware";
    public int Priority => 100;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // 自定义逻辑
        await next(context);
    }

    public bool ShouldExecute(HttpContext context)
    {
        return true;
    }
}
```

### 添加自定义事件处理器

```csharp
public class CustomEventHandler : IEventHandler<CustomEvent>
{
    public string Name => "CustomEventHandler";
    public int Priority => 100;

    public async Task HandleAsync(CustomEvent @event)
    {
        // 处理事件
    }

    public bool ShouldHandle(CustomEvent @event)
    {
        return true;
    }
}
```

### 添加自定义策略

```csharp
public class CustomStrategy : IStrategy<bool>
{
    public string Name => "CustomStrategy";
    public string Id => "custom-strategy";
    public int Priority => 100;

    public async Task<bool> ExecuteAsync(params object[] parameters)
    {
        // 执行策略逻辑
        return true;
    }

    public bool CanExecute(params object[] parameters)
    {
        return parameters.Length > 0;
    }
}
```

## 🧪 运行示例

```bash
cd samples/Framework.Samples
dotnet run
```

程序将自动演示所有 17 种设计模式的使用示例，包括：
- **创建型模式**: 单例、工厂、建造者等
- **结构型模式**: 装饰器、代理、外观等  
- **行为型模式**: 策略、观察者、命令、状态、访问者、迭代器、中介者、备忘录、模板方法等

详细的设计模式说明和示例代码请查看 [DESIGN_PATTERNS.md](./DESIGN_PATTERNS.md)。

访问 `https://localhost:5001/swagger` 查看API文档。

## 📚 设计模式完整文档

本框架实现了 17 种经典设计模式，每种模式都有完整的示例代码：

### 创建型模式（5种）
- ✅ [单例模式](./DESIGN_PATTERNS.md#1-单例模式-singleton) - 3种实现方式（饿汉式、懒汉式、Lazy<T>）
- ✅ [简单工厂模式](./DESIGN_PATTERNS.md#2-简单工厂模式-simple-factory) - 通知系统示例
- ✅ [工厂方法模式](./DESIGN_PATTERNS.md#3-工厂方法模式-factory-method) - 日志系统示例
- ✅ [抽象工厂模式](./DESIGN_PATTERNS.md#4-抽象工厂模式-abstract-factory) - UI组件系统示例
- ✅ [建造者模式](./DESIGN_PATTERNS.md#5-建造者模式-builder) - 电脑配置和HTTP请求构建

### 结构型模式（3种）
- ✅ [装饰器模式](./DESIGN_PATTERNS.md#6-装饰器模式-decorator) - 缓存、性能监控、异常处理、审计日志
- ✅ [代理模式](./DESIGN_PATTERNS.md#7-代理模式-proxy) - 动态代理和拦截器
- ✅ [外观模式](./DESIGN_PATTERNS.md#8-外观模式-facade) - ApplicationFramework统一接口

### 行为型模式（9种）
- ✅ [策略模式](./DESIGN_PATTERNS.md#9-策略模式-strategy) - 验证策略系统
- ✅ [观察者模式](./DESIGN_PATTERNS.md#10-观察者模式-observer) - 事件总线系统
- ✅ [命令模式](./DESIGN_PATTERNS.md#11-命令模式-command) - 命令总线和CQRS
- ✅ [状态模式](./DESIGN_PATTERNS.md#12-状态模式-state) - 用户状态管理
- ✅ [访问者模式](./DESIGN_PATTERNS.md#13-访问者模式-visitor) - 对象审计访问
- ✅ [迭代器模式](./DESIGN_PATTERNS.md#14-迭代器模式-iterator) - 集合遍历和过滤
- ✅ [中介者模式](./DESIGN_PATTERNS.md#15-中介者模式-mediator) - 订单支付通知系统
- ✅ [备忘录模式](./DESIGN_PATTERNS.md#16-备忘录模式-memento) - 文档编辑器和游戏存档
- ✅ [模板方法模式](./DESIGN_PATTERNS.md#17-模板方法模式-template-method) - 用户注册和订单处理流程

**查看完整文档**: [DESIGN_PATTERNS.md](./DESIGN_PATTERNS.md)

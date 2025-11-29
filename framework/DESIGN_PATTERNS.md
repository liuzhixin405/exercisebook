# Framework 设计模式示例文档

本项目展示了在 Framework 中实现的 17 种经典设计模式，涵盖创建型、结构型和行为型三大类。

## 📋 目录

- [创建型模式](#创建型模式)
  - [单例模式](#1-单例模式-singleton)
  - [简单工厂模式](#2-简单工厂模式-simple-factory)
  - [工厂方法模式](#3-工厂方法模式-factory-method)
  - [抽象工厂模式](#4-抽象工厂模式-abstract-factory)
  - [建造者模式](#5-建造者模式-builder)
- [结构型模式](#结构型模式)
  - [装饰器模式](#6-装饰器模式-decorator)
  - [代理模式](#7-代理模式-proxy)
  - [外观模式](#8-外观模式-facade)
- [行为型模式](#行为型模式)
  - [策略模式](#9-策略模式-strategy)
  - [观察者模式](#10-观察者模式-observer)
  - [命令模式](#11-命令模式-command)
  - [状态模式](#12-状态模式-state)
  - [访问者模式](#13-访问者模式-visitor)
  - [迭代器模式](#14-迭代器模式-iterator)
  - [中介者模式](#15-中介者模式-mediator)
  - [备忘录模式](#16-备忘录模式-memento)
  - [模板方法模式](#17-模板方法模式-template-method)

---

## 创建型模式

创建型模式关注对象的创建机制，旨在以适合情况的方式创建对象。

### 1. 单例模式 (Singleton)

**位置**: `samples/Framework.Samples/Singletons/SingletonExamples.cs`

**说明**: 确保一个类只有一个实例，并提供全局访问点。

**实现方式**:
- **饿汉式**: `ConfigurationManager` - 类加载时即创建实例
- **懒汉式（双重检查锁）**: `LogManager` - 首次访问时创建，线程安全
- **Lazy<T>**: `DatabaseConnectionPool` - 使用 .NET 提供的线程安全懒加载

**示例**:
```csharp
// 配置管理器 - 饿汉式
var config = ConfigurationManager.Instance;
config.SetSetting("Theme", "Dark");

// 日志管理器 - 懒汉式
var logger = LogManager.Instance;
logger.Log("应用程序启动");

// 数据库连接池 - Lazy<T>
var dbPool = DatabaseConnectionPool.Instance;
var conn = dbPool.GetConnection();
```

**优点**:
- 全局唯一实例，节省内存
- 全局访问点，方便使用
- 延迟初始化（懒汉式）

### 2. 简单工厂模式 (Simple Factory)

**位置**: `samples/Framework.Samples/Factories/FactoryExamples.cs`

**说明**: 提供一个创建对象的接口，由工厂决定实例化哪个类。

**示例**:
```csharp
var emailNotification = NotificationFactory.CreateNotification(NotificationChannel.Email);
await emailNotification.SendAsync("user@example.com", "欢迎注册！");

var smsNotification = NotificationFactory.CreateNotification(NotificationChannel.SMS);
await smsNotification.SendAsync("13800138000", "验证码: 123456");
```

**优点**:
- 客户端无需知道具体类名
- 集中管理对象创建
- 易于扩展新产品

### 3. 工厂方法模式 (Factory Method)

**位置**: `samples/Framework.Samples/Factories/FactoryExamples.cs`

**说明**: 定义创建对象的接口，让子类决定实例化哪个类。

**示例**:
```csharp
LoggerFactory fileLoggerFactory = new FileLoggerFactory();
fileLoggerFactory.LogMessage("文件日志测试");

LoggerFactory cloudLoggerFactory = new CloudLoggerFactory();
cloudLoggerFactory.LogMessage("云日志测试");
```

**优点**:
- 符合开闭原则
- 更好的扩展性
- 解耦对象的创建和使用

### 4. 抽象工厂模式 (Abstract Factory)

**位置**: `samples/Framework.Samples/Factories/FactoryExamples.cs`

**说明**: 提供一个创建一系列相关或相互依赖对象的接口，而无需指定它们具体的类。

**示例**:
```csharp
IUIFactory windowsFactory = new WindowsUIFactory();
var winButton = windowsFactory.CreateButton();
var winTextBox = windowsFactory.CreateTextBox();

IUIFactory macFactory = new MacUIFactory();
var macButton = macFactory.CreateButton();
var macTextBox = macFactory.CreateTextBox();
```

**优点**:
- 产品族一致性
- 易于切换产品系列
- 符合依赖倒置原则

### 5. 建造者模式 (Builder)

**位置**: `samples/Framework.Samples/Builders/BuilderExamples.cs`

**说明**: 将复杂对象的构建与其表示分离，使得同样的构建过程可以创建不同的表示。

**示例**:
```csharp
// 使用指挥者构建预定义配置
var director = new ComputerDirector();
var gamingPC = director.BuildGamingComputer(new ComputerBuilder());

// 自定义构建
var httpRequest = HttpRequestBuilder.Post("https://api.example.com/users")
    .AddHeader("Content-Type", "application/json")
    .AddHeader("Authorization", "Bearer token123")
    .SetBody("{\"name\":\"张三\"}")
    .Build();
```

**优点**:
- 链式调用，代码清晰
- 可以精细控制构建过程
- 易于创建不同表示的对象

---

## 结构型模式

结构型模式关注类和对象的组合，描述如何将类或对象结合在一起形成更大的结构。

### 6. 装饰器模式 (Decorator)

**位置**: 
- `samples/Framework.Samples/Decorators/` - 具体实现
- `src/Framework.Infrastructure/Decorators/` - 框架支持

**说明**: 动态地给对象添加额外的职责，比继承更灵活。

**示例**:
```csharp
// 缓存装饰器
var cacheProvider = new CacheProvider();
cacheProvider.Set("user:123", "张三", TimeSpan.FromMinutes(5));

// 性能监控装饰器
var perfMonitor = new PerformanceMonitor();
using (perfMonitor.StartMonitoring("数据库查询"))
{
    // 执行操作
}

// 异常处理装饰器
var exceptionHandler = new ExceptionHandler();
await exceptionHandler.HandleExceptionAsync(exception);

// 审计日志装饰器
var auditLogger = new AuditLogger();
await auditLogger.LogAuditAsync("用户登录", details);
```

**优点**:
- 比继承更灵活
- 可以动态组合功能
- 符合单一职责原则

### 7. 代理模式 (Proxy)

**位置**: `src/Framework.Infrastructure/Proxies/ProxyFactory.cs`

**说明**: 为其他对象提供代理以控制对这个对象的访问。

**框架实现**:
- 动态代理生成
- 拦截器支持
- AOP 功能

**示例**:
```csharp
var proxy = framework.ProxyFactory.CreateProxy<IUserService>(
    userService,
    new LoggingInterceptor(),
    new CachingInterceptor()
);
```

**优点**:
- 控制对象访问
- 添加额外功能（日志、缓存等）
- 远程代理、虚拟代理、保护代理等多种用途

### 8. 外观模式 (Facade)

**位置**: `src/Framework.Infrastructure/ApplicationFramework.cs`

**说明**: 为子系统中的一组接口提供统一的接口，定义了一个高层接口使子系统更容易使用。

**框架实现**:
```csharp
public class ApplicationFramework : IApplicationFramework
{
    public IServiceContainer ServiceContainer { get; }
    public IEventBus EventBus { get; }
    public ICommandBus CommandBus { get; }
    public IStateManager StateManager { get; }
    public IStrategyContext StrategyContext { get; }
    public IProxyFactory ProxyFactory { get; }
    public IVisitorRegistry VisitorRegistry { get; }
    // ... 统一接口
}
```

**优点**:
- 简化复杂系统的使用
- 降低客户端与子系统的耦合
- 更好的分层结构

---

## 行为型模式

行为型模式关注对象之间的通信，描述对象之间怎样交互和怎样分配职责。

### 9. 策略模式 (Strategy)

**位置**: 
- `samples/Framework.Samples/Strategies/` - 具体策略
- `src/Framework.Infrastructure/Strategies/` - 框架支持

**说明**: 定义一系列算法，把它们封装起来，并使它们可以相互替换。

**示例**:
```csharp
// 邮箱验证策略
var emailStrategy = framework.StrategyContext.GetStrategy<EmailValidationStrategy>();
var isValid = await emailStrategy.ExecuteAsync("test@example.com");

// 密码验证策略
var passwordStrategy = framework.StrategyContext.GetStrategy<PasswordValidationStrategy>();
var isStrongPassword = await passwordStrategy.ExecuteAsync("MyP@ssw0rd");
```

**优点**:
- 算法可以自由切换
- 避免多重条件判断
- 扩展性好

### 10. 观察者模式 (Observer)

**位置**: `src/Framework.Infrastructure/Events/EventBus.cs`

**说明**: 定义对象间一对多的依赖关系，当一个对象状态改变时，所有依赖它的对象都会得到通知。

**示例**:
```csharp
// 订阅事件
framework.EventBus.Subscribe(new UserCreatedEventHandler());

// 发布事件
var userCreatedEvent = new UserCreatedEvent 
{ 
    UserId = Guid.NewGuid(), 
    UserName = "张三" 
};
await framework.EventBus.PublishAsync(userCreatedEvent);
```

**优点**:
- 松耦合
- 支持广播通信
- 符合开闭原则

### 11. 命令模式 (Command)

**位置**: 
- `samples/Framework.Samples/Commands/` - 具体命令
- `src/Framework.Infrastructure/Commands/CommandBus.cs` - 命令总线

**说明**: 将请求封装成对象，从而使你可用不同的请求对客户进行参数化。

**示例**:
```csharp
// 创建用户命令
var createUserCommand = new CreateUserCommand 
{ 
    UserName = "李四", 
    Email = "lisi@example.com" 
};
await framework.CommandBus.SendAsync(createUserCommand);

// 更新用户命令
var updateUserCommand = new UpdateUserCommand 
{ 
    UserId = userId, 
    NewName = "李四四" 
};
await framework.CommandBus.SendAsync(updateUserCommand);
```

**优点**:
- 解耦请求发送者和接收者
- 支持撤销/重做
- 支持命令队列和日志

### 12. 状态模式 (State)

**位置**: 
- `samples/Framework.Samples/States/` - 具体状态
- `src/Framework.Infrastructure/States/StateManager.cs` - 状态管理器

**说明**: 允许对象在内部状态改变时改变它的行为。

**示例**:
```csharp
// 用户注册状态
var registrationState = new UserRegistrationState();
framework.StateManager.SetState(registrationState);

// 状态转换
var activeState = new UserActiveState();
framework.StateManager.TransitionTo(activeState);

// 获取当前状态
var currentState = framework.StateManager.GetCurrentState<UserActiveState>();
```

**优点**:
- 封装状态转换逻辑
- 避免大量条件判断
- 易于添加新状态

### 13. 访问者模式 (Visitor)

**位置**: 
- `samples/Framework.Samples/Visitors/` - 具体访问者
- `src/Framework.Infrastructure/Visitors/VisitorRegistry.cs` - 访问者注册器

**说明**: 表示作用于某对象结构中的各元素的操作，使你可以在不改变各元素类的前提下定义作用于这些元素的新操作。

**示例**:
```csharp
// 注册访问者
framework.VisitorRegistry.Register(new UserAuditVisitor());

// 访问对象
var user = new User { Id = userId, Name = "王五" };
await framework.VisitorRegistry.VisitAsync(user);
```

**优点**:
- 增加新操作很容易
- 相关行为集中在访问者
- 符合单一职责原则

### 14. 迭代器模式 (Iterator)

**位置**: 
- `samples/Framework.Samples/Iterators/` - 迭代器实现
- `src/Framework.Infrastructure/Iterators/` - 框架支持

**说明**: 提供一种方法顺序访问聚合对象中的各个元素，而又不暴露其内部表示。

**示例**:
```csharp
var userCollection = new UserCollection();
userCollection.AddUser(new UserData { Name = "用户1", Role = "Admin" });
userCollection.AddUser(new UserData { Name = "用户2", Role = "User" });

// 基本迭代
var iterator = userCollection.GetIterator();
while (iterator.MoveNext())
{
    var user = iterator.Current;
    Console.WriteLine(user.Name);
}

// 过滤迭代器
var adminIterator = new FilterIterator<UserData>(
    iterator, 
    u => u.Role == "Admin"
);
while (adminIterator.MoveNext())
{
    var admin = adminIterator.Current;
    Console.WriteLine(admin.Name);
}
```

**优点**:
- 分离聚合对象的遍历行为
- 简化聚合类
- 支持多种遍历方式

### 15. 中介者模式 (Mediator)

**位置**: 
- `samples/Framework.Samples/Mediators/` - 消息和处理器
- `src/Framework.Infrastructure/Mediators/Mediator.cs` - 中介者实现

**说明**: 用一个中介对象封装一系列对象交互，使各对象不需要显式地相互引用。

**示例**:
```csharp
var mediator = new Mediator();

// 注册处理器
mediator.RegisterHandler(new OrderMessageHandler());
mediator.RegisterHandler(new PaymentMessageHandler());

// 发送消息
var orderMsg = new OrderMessage 
{ 
    OrderId = Guid.NewGuid(), 
    ProductName = "笔记本电脑" 
};
await mediator.SendAsync(orderMsg);

// 发送消息并获取结果
var paymentMsg = new PaymentMessage { Amount = 5999 };
var result = await mediator.SendAsync<PaymentMessage, PaymentResult>(paymentMsg);
```

**优点**:
- 减少对象间的耦合
- 集中控制交互
- 简化对象协议

### 16. 备忘录模式 (Memento)

**位置**: 
- `samples/Framework.Samples/Mementos/` - 具体应用
- `src/Framework.Infrastructure/Memento/` - 框架支持

**说明**: 在不破坏封装性的前提下，捕获对象的内部状态并在该对象之外保存这个状态。

**示例**:
```csharp
var mementoManager = new MementoManager();
var docEditor = new DocumentEditor(mementoManager);

// 编辑文档
docEditor.Title = "我的文档";
docEditor.Content = "第一版内容";
var save1 = docEditor.Save();

// 继续编辑
docEditor.Content = "第二版内容";
var save2 = docEditor.Save();

// 恢复到之前的版本
docEditor.Restore(save1);

// 游戏存档示例
var gameManager = new GameSaveManager(mementoManager);
gameManager.Play();
var saveId = gameManager.SaveGame();
gameManager.LoadGame(saveId);
```

**优点**:
- 保存和恢复状态
- 实现撤销功能
- 保持封装边界

### 17. 模板方法模式 (Template Method)

**位置**: 
- `samples/Framework.Samples/Templates/` - 具体模板
- `src/Framework.Infrastructure/Templates/` - 模板基类

**说明**: 定义算法骨架，将某些步骤延迟到子类，使得子类可以不改变算法结构的情况下重定义某些步骤。

**示例**:
```csharp
// 用户注册模板
var registrationTemplate = new UserRegistrationTemplate();
var context = new UserRegistrationContext
{
    UserName = "测试用户",
    Email = "test@example.com",
    Password = "pass123456"
};
var result = await registrationTemplate.ExecuteAsync(context);

// 订单处理模板
var orderTemplate = new OrderProcessingTemplate();
var orderContext = new OrderContext
{
    CustomerName = "客户A",
    Items = orderItems,
    TotalAmount = 399.97m
};
var orderResult = await orderTemplate.ExecuteAsync(orderContext);
```

**模板方法流程**:
1. `InitializeAsync()` - 初始化
2. `ValidateAsync()` - 验证
3. `ProcessAsync()` - 处理（抽象，子类实现）
4. `CleanupAsync()` - 清理
5. `OnErrorAsync()` - 错误处理

**优点**:
- 代码复用
- 控制子类扩展点
- 符合开闭原则

---

## 🚀 运行示例

```bash
cd samples/Framework.Samples
dotnet run
```

程序将按顺序演示所有 17 种设计模式的使用。

## 📊 设计模式总结

### 创建型模式（5种）
- ✅ 单例模式 - 全局唯一实例
- ✅ 简单工厂 - 集中创建对象
- ✅ 工厂方法 - 子类决定实例化
- ✅ 抽象工厂 - 创建产品族
- ✅ 建造者 - 复杂对象构建

### 结构型模式（3种）
- ✅ 装饰器 - 动态添加功能
- ✅ 代理 - 控制对象访问
- ✅ 外观 - 简化子系统接口

### 行为型模式（9种）
- ✅ 策略 - 算法可互换
- ✅ 观察者 - 一对多依赖
- ✅ 命令 - 请求封装
- ✅ 状态 - 状态改变行为
- ✅ 访问者 - 操作与对象分离
- ✅ 迭代器 - 顺序访问元素
- ✅ 中介者 - 集中控制交互
- ✅ 备忘录 - 保存恢复状态
- ✅ 模板方法 - 定义算法骨架

## 💡 最佳实践

1. **选择合适的模式**: 不要为了使用模式而使用模式，要根据实际需求选择
2. **保持简单**: 简单的解决方案优于复杂的设计模式
3. **组合使用**: 多个模式可以组合使用以解决复杂问题
4. **遵循原则**: SOLID 原则应该指导设计模式的应用
5. **代码可读性**: 使用模式应该提高而不是降低代码可读性

## 📚 参考资料

- 《设计模式：可复用面向对象软件的基础》(GoF)
- 《Head First 设计模式》
- Microsoft 官方文档：https://docs.microsoft.com/zh-cn/dotnet/architecture/

---

**注意**: 本示例仅用于学习目的，展示了各种设计模式的基本实现。在实际项目中，应根据具体需求进行调整和优化。

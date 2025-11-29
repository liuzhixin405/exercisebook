# 设计模式快速参考

本文档提供所有 17 种设计模式的快速参考和代码片段。

## 📑 快速导航

| 模式 | 分类 | 用途 | 文件位置 |
|------|------|------|----------|
| 单例 | 创建型 | 全局唯一实例 | `Singletons/SingletonExamples.cs` |
| 简单工厂 | 创建型 | 集中创建对象 | `Factories/FactoryExamples.cs` |
| 工厂方法 | 创建型 | 子类决定实例化 | `Factories/FactoryExamples.cs` |
| 抽象工厂 | 创建型 | 创建产品族 | `Factories/FactoryExamples.cs` |
| 建造者 | 创建型 | 复杂对象构建 | `Builders/BuilderExamples.cs` |
| 装饰器 | 结构型 | 动态添加功能 | `Decorators/` |
| 代理 | 结构型 | 控制对象访问 | `Infrastructure/Proxies/` |
| 外观 | 结构型 | 简化接口 | `Infrastructure/ApplicationFramework.cs` |
| 策略 | 行为型 | 算法可互换 | `Strategies/` |
| 观察者 | 行为型 | 一对多通知 | `Infrastructure/Events/` |
| 命令 | 行为型 | 请求封装 | `Commands/` |
| 状态 | 行为型 | 状态改变行为 | `States/` |
| 访问者 | 行为型 | 操作与对象分离 | `Visitors/` |
| 迭代器 | 行为型 | 顺序访问 | `Iterators/` |
| 中介者 | 行为型 | 集中控制交互 | `Mediators/` |
| 备忘录 | 行为型 | 保存恢复状态 | `Mementos/` |
| 模板方法 | 行为型 | 算法骨架 | `Templates/` |

## 🎯 使用场景速查

### 需要全局唯一实例？
→ **单例模式** `ConfigurationManager.Instance`

### 需要创建不同类型的对象？
→ **简单工厂** `NotificationFactory.CreateNotification(type)`  
→ **工厂方法** `new FileLoggerFactory().CreateLogger()`  
→ **抽象工厂** `uiFactory.CreateButton()`

### 需要构建复杂对象？
→ **建造者** `builder.SetCPU().SetRAM().Build()`

### 需要动态添加功能？
→ **装饰器** `new CacheProvider()`, `new PerformanceMonitor()`

### 需要控制对象访问？
→ **代理** `proxyFactory.CreateProxy<T>()`

### 需要简化复杂子系统？
→ **外观** `ApplicationFramework`

### 需要切换算法？
→ **策略** `strategyContext.GetStrategy<T>()`

### 需要通知多个对象？
→ **观察者** `eventBus.PublishAsync(event)`

### 需要封装请求？
→ **命令** `commandBus.SendAsync(command)`

### 对象行为依赖于状态？
→ **状态** `stateManager.TransitionTo(state)`

### 需要对对象结构进行操作？
→ **访问者** `visitorRegistry.VisitAsync(obj)`

### 需要遍历集合？
→ **迭代器** `iterator.MoveNext()`

### 对象间需要协调交互？
→ **中介者** `mediator.SendAsync(message)`

### 需要撤销/恢复状态？
→ **备忘录** `mementoManager.SaveState()` / `RestoreState()`

### 需要定义算法步骤？
→ **模板方法** `template.ExecuteAsync(context)`

## 💻 代码片段

### 单例模式
```csharp
// 获取单例实例
var config = ConfigurationManager.Instance;
var logger = LogManager.Instance;
var pool = DatabaseConnectionPool.Instance;
```

### 工厂模式
```csharp
// 简单工厂
var notification = NotificationFactory.CreateNotification(NotificationChannel.Email);

// 工厂方法
LoggerFactory factory = new CloudLoggerFactory();
var logger = factory.CreateLogger();

// 抽象工厂
IUIFactory factory = new WindowsUIFactory();
var button = factory.CreateButton();
```

### 建造者模式
```csharp
var computer = new ComputerBuilder()
    .SetCPU("Intel i9")
    .SetRAM("32GB")
    .SetGPU("RTX 4090")
    .Build();

var request = HttpRequestBuilder.Post("https://api.com")
    .AddHeader("Auth", "token")
    .SetBody(json)
    .Build();
```

### 装饰器模式
```csharp
var cache = new CacheProvider();
cache.Set("key", value, TimeSpan.FromMinutes(5));

var monitor = new PerformanceMonitor();
using (monitor.StartMonitoring("operation")) 
{
    // 操作
}
```

### 策略模式
```csharp
var strategy = strategyContext.GetStrategy<EmailValidationStrategy>();
var isValid = await strategy.ExecuteAsync(email);
```

### 观察者模式
```csharp
// 发布事件
await eventBus.PublishAsync(new UserCreatedEvent 
{ 
    UserId = id, 
    UserName = name 
});
```

### 命令模式
```csharp
// 执行命令
await commandBus.SendAsync(new CreateUserCommand 
{ 
    UserName = "user", 
    Email = "email@test.com" 
});
```

### 状态模式
```csharp
// 设置状态
stateManager.SetState(new UserActiveState());

// 转换状态
stateManager.TransitionTo(new UserSuspendedState());
```

### 访问者模式
```csharp
// 注册访问者
visitorRegistry.Register(new UserAuditVisitor());

// 访问对象
await visitorRegistry.VisitAsync(user);
```

### 迭代器模式
```csharp
var iterator = collection.GetIterator();
while (iterator.MoveNext())
{
    var item = iterator.Current;
    // 处理项
}

// 过滤迭代
var filtered = new FilterIterator<T>(iterator, item => item.IsActive);
```

### 中介者模式
```csharp
var mediator = new Mediator();
mediator.RegisterHandler(new OrderMessageHandler());

await mediator.SendAsync(new OrderMessage { ... });

var result = await mediator.SendAsync<PaymentMessage, PaymentResult>(payment);
```

### 备忘录模式
```csharp
var manager = new MementoManager();
var editor = new DocumentEditor(manager);

// 保存状态
var saveId = editor.Save();

// 恢复状态
editor.Restore(saveId);
```

### 模板方法模式
```csharp
var template = new UserRegistrationTemplate();
var result = await template.ExecuteAsync(new UserRegistrationContext
{
    UserName = "user",
    Email = "email@test.com",
    Password = "pass123"
});
```

## 🔍 选择指南

### 按问题选择

**创建对象时**:
- 需要唯一实例 → 单例
- 创建逻辑复杂 → 工厂/建造者
- 产品族一致 → 抽象工厂

**组合对象时**:
- 添加功能 → 装饰器
- 控制访问 → 代理
- 简化接口 → 外观

**对象交互时**:
- 算法变化 → 策略
- 通知多个对象 → 观察者
- 请求参数化 → 命令
- 状态影响行为 → 状态
- 操作与结构分离 → 访问者
- 遍历集合 → 迭代器
- 协调交互 → 中介者
- 撤销/重做 → 备忘录
- 算法步骤固定 → 模板方法

## ⚠️ 常见误区

1. **过度使用**: 不要为了用模式而用模式
2. **模式组合**: 多个模式可以组合使用
3. **简单优先**: 简单方案优于复杂模式
4. **灵活变通**: 根据需要调整模式实现

## 📖 更多信息

详细说明请查看:
- [完整设计模式文档](./DESIGN_PATTERNS.md)
- [框架主文档](./README.md)
- 示例代码: `samples/Framework.Samples/`

---

**快速开始**: 
```bash
cd samples/Framework.Samples
dotnet run
```

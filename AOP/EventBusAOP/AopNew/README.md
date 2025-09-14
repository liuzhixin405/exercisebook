# CommandBus AOP 项目

这是一个基于AOP（面向切面编程）的CommandBus项目，使用TPL Dataflow进行数据流处理优化，支持多种CommandBus实现和实时监控。

## 项目结构

```
AopNew/
├── Common.Bus/                    # 核心库
│   ├── Core/                      # 核心接口和抽象
│   │   ├── ICommand.cs           # 命令接口
│   │   ├── ICommandBus.cs        # 命令总线接口
│   │   ├── ICommandHandler.cs    # 命令处理器接口
│   │   ├── ICommandPipelineBehavior.cs # 管道行为接口
│   │   ├── ICommandProcessor.cs  # 命令处理器接口
│   │   ├── ICommandRequest.cs    # 命令请求接口
│   │   └── CommandBusType.cs     # CommandBus类型枚举
│   ├── Implementations/          # 具体实现
│   │   ├── CommandBus.cs         # 标准CommandBus
│   │   ├── DataflowCommandBus.cs # TPL Dataflow CommandBus
│   │   ├── BatchDataflowCommandBus.cs # 批处理Dataflow CommandBus
│   │   ├── TypedDataflowCommandBus.cs # 类型安全Dataflow CommandBus
│   │   ├── MonitoredCommandBus.cs # 带监控的CommandBus
│   │   ├── CommandBusFactory.cs  # CommandBus工厂（已废弃）
│   │   └── CommandBusServiceLocator.cs # 服务定位器
│   ├── Monitoring/               # 监控相关
│   │   ├── IDataflowMetrics.cs   # 数据流指标接口
│   │   ├── IMetricsCollector.cs  # 指标收集器接口
│   │   ├── DataflowMetrics.cs    # 数据流指标实现
│   │   └── BatchDataflowMetrics.cs # 批处理指标实现
│   └── Extensions/               # 扩展方法
│       └── ServiceCollectionExtensions.cs # DI扩展方法
└── WebApp/                       # Web应用程序
    ├── Commands/                 # 命令定义
    │   ├── ProcessOrderCommand.cs
    │   ├── CreateUserCommand.cs
    │   └── SendEmailCommand.cs
    ├── Handlers/                 # 命令处理器
    │   ├── ProcessOrderHandler.cs
    │   ├── CreateUserHandler.cs
    │   └── SendEmailHandler.cs
    ├── Behaviors/                # 管道行为
    │   ├── LoggingBehavior.cs
    │   ├── ValidationBehavior.cs
    │   └── TransactionBehavior.cs
    ├── Controllers/              # API控制器
    │   ├── StandardCommandBusController.cs
    │   ├── DataflowCommandBusController.cs
    │   ├── BatchDataflowCommandBusController.cs
    │   ├── TypedDataflowCommandBusController.cs
    │   ├── MonitoredCommandBusController.cs
    │   ├── CommandBusDemoController.cs
    │   └── MonitoringController.cs
    └── Models/                   # 数据模型
        └── WeatherForecast.cs
```

## CommandBus实现类型

### 1. Standard CommandBus
- **类型**: `CommandBusType.Standard`
- **特点**: 标准同步处理，适合简单场景
- **控制器**: `StandardCommandBusController`

### 2. Dataflow CommandBus
- **类型**: `CommandBusType.Dataflow`
- **特点**: 基于TPL Dataflow的异步并发处理，适合高并发场景
- **控制器**: `DataflowCommandBusController`

### 3. Batch Dataflow CommandBus
- **类型**: `CommandBusType.BatchDataflow`
- **特点**: 支持批量处理，适合大批量数据场景
- **控制器**: `BatchDataflowCommandBusController`

### 4. Typed Dataflow CommandBus
- **类型**: `CommandBusType.TypedDataflow`
- **特点**: 强类型安全，适合复杂业务场景
- **控制器**: `TypedDataflowCommandBusController`

### 5. Monitored CommandBus
- **类型**: `CommandBusType.Monitored`
- **特点**: 包含性能监控，适合生产环境
- **控制器**: `MonitoredCommandBusController`

## 使用方法

### 1. 依赖注入配置

在`Program.cs`中一次性注册所有CommandBus实现：

```csharp
// 一次性注册所有CommandBus实现
builder.Services.AddAllCommandBusImplementations();

// 注册命令处理器
builder.Services.AddScoped<ICommandHandler<ProcessOrderCommand, string>, ProcessOrderHandler>();
builder.Services.AddScoped<ICommandHandler<CreateUserCommand, int>, CreateUserHandler>();
builder.Services.AddScoped<ICommandHandler<SendEmailCommand, bool>, SendEmailHandler>();

// 注册管道行为
builder.Services.AddScoped(typeof(ICommandPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddScoped(typeof(ICommandPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddScoped(typeof(ICommandPipelineBehavior<,>), typeof(TransactionBehavior<,>));
```

### 2. 在控制器中使用

每个控制器直接注入对应的CommandBus实现：

```csharp
public class StandardCommandBusController : ControllerBase
{
    private readonly CommandBus _commandBus;

    public StandardCommandBusController(CommandBus commandBus)
    {
        _commandBus = commandBus;
    }

    [HttpPost("process-order")]
    public async Task<IActionResult> ProcessOrder([FromBody] ProcessOrderCommand command)
    {
        var result = await _commandBus.SendAsync<ProcessOrderCommand, string>(command);
        
        return Ok(new { 
            Success = true, 
            Result = result, 
            BusType = "Standard"
        });
    }
}
```

### 3. 专用控制器端点

每个CommandBus实现都有专门的控制器端点：

```bash
# 标准CommandBus
POST /api/StandardCommandBus/process-order
POST /api/StandardCommandBus/create-user
POST /api/StandardCommandBus/send-email

# Dataflow CommandBus
POST /api/DataflowCommandBus/process-order
POST /api/DataflowCommandBus/create-user
POST /api/DataflowCommandBus/send-email

# 批处理Dataflow CommandBus
POST /api/BatchDataflowCommandBus/process-order
POST /api/BatchDataflowCommandBus/create-user
POST /api/BatchDataflowCommandBus/send-email

# 类型安全Dataflow CommandBus
POST /api/TypedDataflowCommandBus/process-order
POST /api/TypedDataflowCommandBus/create-user
POST /api/TypedDataflowCommandBus/send-email

# 带监控的CommandBus
POST /api/MonitoredCommandBus/process-order
POST /api/MonitoredCommandBus/create-user
POST /api/MonitoredCommandBus/send-email
```

## API端点

### 专用控制器端点

每个CommandBus实现都有专门的控制器：

- `StandardCommandBusController` - 标准CommandBus演示
- `DataflowCommandBusController` - TPL Dataflow CommandBus演示
- `BatchDataflowCommandBusController` - 批处理Dataflow CommandBus演示
- `TypedDataflowCommandBusController` - 类型安全Dataflow CommandBus演示
- `MonitoredCommandBusController` - 带监控的CommandBus演示

### 监控端点

- `GET /api/Monitoring/dashboard` - 监控面板
- `GET /api/Monitoring/stream` - SSE实时数据流
- `GET /api/Monitoring/metrics` - 获取当前指标

## 示例请求

### 处理订单

```json
POST /api/TypedDataflowCommandBus/process-order
Content-Type: application/json

{
    "product": "笔记本电脑",
    "quantity": 2,
    "priority": 1
}
```

### 创建用户

```json
POST /api/TypedDataflowCommandBus/create-user
Content-Type: application/json

{
    "name": "张三",
    "email": "zhangsan@example.com",
    "age": 25
}
```

### 发送邮件

```json
POST /api/MonitoredCommandBus/send-email
Content-Type: application/json

{
    "to": "user@example.com",
    "subject": "测试邮件",
    "body": "这是一封测试邮件"
}
```

## 运行项目

1. 克隆项目到本地
2. 在项目根目录运行：
   ```bash
   dotnet build
   dotnet run --project WebApp
   ```
3. 访问 `https://localhost:5056` 查看Swagger文档
4. 访问 `https://localhost:5056/api/Monitoring/dashboard` 查看监控面板

## 技术特性

- **多种CommandBus实现**: 支持标准、Dataflow、批处理、类型安全、监控等多种实现
- **枚举驱动选择**: 通过枚举类型轻松切换不同的CommandBus实现
- **AOP支持**: 内置日志、验证、事务等管道行为
- **实时监控**: 支持SSE实时数据流和性能指标监控
- **类型安全**: 强类型命令和处理器，编译时类型检查
- **高并发**: 基于TPL Dataflow的异步并发处理
- **批量处理**: 支持批量命令处理，提高吞吐量
- **依赖注入**: 完整的DI支持，易于测试和扩展

## 扩展指南

### 添加新的CommandBus实现

1. 实现`ICommandBus`接口
2. 在`ServiceCollectionExtensions.AddAllCommandBusImplementations`中注册
3. 创建对应的控制器

### 添加新的管道行为

1. 实现`ICommandPipelineBehavior<TCommand, TResult>`接口
2. 在`Program.cs`中注册服务
3. 行为将自动应用到所有命令处理

### 添加新的命令和处理器

1. 在`Commands`目录中定义命令
2. 在`Handlers`目录中实现处理器
3. 在`Program.cs`中注册服务

## 许可证

MIT License

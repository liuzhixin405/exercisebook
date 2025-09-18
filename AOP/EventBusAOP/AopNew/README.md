# CommandBus AOP 项目

这是一个基于AOP（面向切面编程）的CommandBus项目，使用TPL Dataflow进行数据流处理优化，支持多种CommandBus实现、完整的AOP横切关注点和实时监控。

## 核心特性

- **完整的AOP支持**: 参数贯穿、方法执行前后、返回值贯穿、异常处理
- **多种CommandBus实现**: 标准、Dataflow、批处理、类型安全、监控、增强版
- **实时监控**: SSE数据流和性能指标监控
- **类型安全**: 强类型命令和处理器，编译时类型检查
- **高并发**: 基于TPL Dataflow的异步并发处理
- **批量处理**: 支持批量命令处理，提高吞吐量

## 项目结构

```
AopNew/
├── Common.Bus/                    # 核心库
│   ├── Core/                      # 核心接口和抽象
│   │   ├── ICommand.cs           # 命令接口
│   │   ├── ICommandBus.cs        # 命令总线接口
│   │   ├── ICommandHandler.cs    # 命令处理器接口
│   │   ├── ICommandPipelineBehavior.cs # 管道行为接口（包含完整的AOP接口）
│   │   ├── ICommandProcessor.cs  # 命令处理器接口
│   │   ├── ICommandRequest.cs    # 命令请求接口
│   │   ├── EnhancedPipelineExecutor.cs # 增强的管道执行器
│   │   ├── EnhancedCommandBus.cs # 增强的命令总线
│   │   └── CommandBusType.cs     # CommandBus类型枚举
│   ├── Implementations/          # 具体实现
│   │   ├── CommandBus.cs         # 标准CommandBus
│   │   ├── DataflowCommandBus.cs # TPL Dataflow CommandBus
│   │   ├── BatchDataflowCommandBus.cs # 批处理Dataflow CommandBus
│   │   ├── TypedDataflowCommandBus.cs # 类型安全Dataflow CommandBus
│   │   ├── MonitoredCommandBus.cs # 带监控的CommandBus
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
    │   ├── LoggingBehavior.cs    # 传统日志行为
    │   ├── ValidationBehavior.cs # 传统验证行为
    │   ├── TransactionBehavior.cs # 传统事务行为
    │   ├── ParameterValidationBehavior.cs # 参数验证行为（AOP）
    │   ├── PreExecutionLoggingBehavior.cs # 执行前日志行为（AOP）
    │   ├── PostExecutionLoggingBehavior.cs # 执行后日志行为（AOP）
    │   ├── ReturnValueEnhancementBehavior.cs # 返回值增强行为（AOP）
    │   └── ExceptionHandlingBehavior.cs # 异常处理行为（AOP）
    ├── Controllers/              # API控制器
    │   ├── StandardCommandBusController.cs
    │   ├── DataflowCommandBusController.cs
    │   ├── BatchDataflowCommandBusController.cs
    │   ├── TypedDataflowCommandBusController.cs
    │   ├── MonitoredCommandBusController.cs
    │   ├── EnhancedCommandBusController.cs # 增强AOP命令总线控制器
    │   └── MonitoringController.cs
    ├── Program.cs                # 应用程序入口
    ├── WebApp.csproj            # 项目文件
    └── WebApp.http              # HTTP测试文件
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

### 6. Enhanced CommandBus ⭐
- **类型**: `EnhancedCommandBus`
- **特点**: 支持完整的AOP横切关注点，企业级应用首选
- **控制器**: `EnhancedCommandBusController`
- **AOP支持**:
  - 参数贯穿处理 (Parameter Interception)
  - 方法执行前处理 (Pre-Execution)
  - 方法执行后处理 (Post-Execution)
  - 返回值贯穿处理 (Return Value Interception)
  - 异常处理 (Exception Handling)

## 使用方法

### 1. 依赖注入配置

在`Program.cs`中一次性注册所有CommandBus实现：

```csharp
// 一次性注册所有CommandBus实现
builder.Services.AddAllCommandBusImplementations();

// 注册增强的AOP行为
builder.Services.AddEnhancedBehaviors();

// 注册命令处理器
builder.Services.AddScoped<ICommandHandler<ProcessOrderCommand, string>, ProcessOrderHandler>();
builder.Services.AddScoped<ICommandHandler<CreateUserCommand, int>, CreateUserHandler>();
builder.Services.AddScoped<ICommandHandler<SendEmailCommand, bool>, SendEmailHandler>();

// 注册传统管道行为
builder.Services.AddScoped(typeof(ICommandPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddScoped(typeof(ICommandPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddScoped(typeof(ICommandPipelineBehavior<,>), typeof(TransactionBehavior<,>));
```

### 2. 在控制器中使用

每个控制器直接注入对应的CommandBus实现：

```csharp
// 标准CommandBus控制器
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

// 增强CommandBus控制器（支持完整AOP）
public class EnhancedCommandBusController : ControllerBase
{
    private readonly EnhancedCommandBus _commandBus;

    public EnhancedCommandBusController(EnhancedCommandBus commandBus)
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
            BusType = "Enhanced AOP"
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

# 增强AOP CommandBus ⭐
POST /api/EnhancedCommandBus/process-order
POST /api/EnhancedCommandBus/create-user
POST /api/EnhancedCommandBus/send-email
GET /api/EnhancedCommandBus/info
```

## API端点

### CommandBus专用控制器

每个CommandBus实现都有专门的控制器：

- `StandardCommandBusController` - 标准CommandBus演示
- `DataflowCommandBusController` - TPL Dataflow CommandBus演示
- `BatchDataflowCommandBusController` - 批处理Dataflow CommandBus演示
- `TypedDataflowCommandBusController` - 类型安全Dataflow CommandBus演示
- `MonitoredCommandBusController` - 带监控的CommandBus演示
- `EnhancedCommandBusController` ⭐ - 增强AOP CommandBus演示（支持完整的横切关注点）

### 监控控制器

- `MonitoringController` - 实时监控和SSE数据流
  - `GET /api/Monitoring/dashboard` - 监控面板
  - `GET /api/Monitoring/stream` - SSE实时数据流
  - `GET /api/Monitoring/metrics` - 获取当前指标

## 示例请求

### 处理订单（增强AOP版本）

```json
POST /api/EnhancedCommandBus/process-order
Content-Type: application/json

{
    "product": "笔记本电脑",
    "quantity": 2,
    "priority": 1
}
```

**响应示例**：
```json
{
    "message": "订单处理成功",
    "result": "[2024-01-15 10:30:01] 订单处理成功: 笔记本电脑 x2"
}
```

### 创建用户（增强AOP版本）

```json
POST /api/EnhancedCommandBus/create-user
Content-Type: application/json

{
    "name": "张三",
    "email": "zhangsan@example.com",
    "age": 25
}
```

**响应示例**：
```json
{
    "message": "用户创建成功",
    "result": "[2024-01-15 10:30:01] 用户创建成功: 张三 (zhangsan@example.com)"
}
```

### 发送邮件（增强AOP版本）

```json
POST /api/EnhancedCommandBus/send-email
Content-Type: application/json

{
    "to": "user@example.com",
    "subject": "测试邮件",
    "body": "这是一封测试邮件"
}
```

**响应示例**：
```json
{
    "message": "邮件发送成功",
    "result": "[2024-01-15 10:30:01] 邮件发送成功: 测试邮件 -> user@example.com"
}
```

### 获取增强CommandBus信息

```json
GET /api/EnhancedCommandBus/info
```

**响应示例**：
```json
{
    "name": "Enhanced Command Bus",
    "description": "支持完整AOP横切关注点的增强命令总线",
    "features": [
        "参数贯穿处理 (Parameter Interception)",
        "方法执行前处理 (Pre-Execution)",
        "方法执行后处理 (Post-Execution)",
        "返回值贯穿处理 (Return Value Interception)",
        "异常处理 (Exception Handling)",
        "完整的日志记录",
        "类型安全的泛型支持"
    ],
    "pipeline": [
        "1. 参数验证和转换",
        "2. 执行前日志和权限检查",
        "3. 命令处理器执行",
        "4. 执行后日志和结果缓存",
        "5. 返回值增强和格式化",
        "6. 异常捕获和处理"
    ]
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

- **完整的AOP支持**: 参数贯穿、方法执行前后、返回值贯穿、异常处理
- **多种CommandBus实现**: 支持标准、Dataflow、批处理、类型安全、监控、增强版等多种实现
- **实时监控**: 支持SSE实时数据流和性能指标监控
- **类型安全**: 强类型命令和处理器，编译时类型检查
- **高并发**: 基于TPL Dataflow的异步并发处理
- **批量处理**: 支持批量命令处理，提高吞吐量
- **依赖注入**: 完整的DI支持，易于测试和扩展
- **企业级特性**: 完整的日志记录、异常处理、性能监控

## 扩展指南

### 添加新的CommandBus实现

1. 实现`ICommandBus`接口
2. 在`ServiceCollectionExtensions.AddAllCommandBusImplementations`中注册
3. 创建对应的控制器

### 添加新的管道行为

#### 传统管道行为
1. 实现`ICommandPipelineBehavior<TCommand, TResult>`接口
2. 在`Program.cs`中注册服务
3. 行为将自动应用到所有命令处理

#### 增强AOP行为
1. 实现对应的AOP接口：
   - `IParameterInterceptionBehavior<TCommand, TResult>` - 参数贯穿
   - `IPreExecutionBehavior<TCommand, TResult>` - 执行前处理
   - `IPostExecutionBehavior<TCommand, TResult>` - 执行后处理
   - `IReturnValueInterceptionBehavior<TCommand, TResult>` - 返回值贯穿
   - `IExceptionHandlingBehavior<TCommand, TResult>` - 异常处理
2. 在`ServiceCollectionExtensions.AddEnhancedBehaviors`中注册
3. 行为将自动应用到增强CommandBus

### 添加新的命令和处理器

1. 在`Commands`目录中定义命令
2. 在`Handlers`目录中实现处理器
3. 在`Program.cs`中注册服务

## AOP管道执行流程

### 增强CommandBus的完整执行流程

```
命令输入 (TCommand)
    ↓
1. 参数贯穿处理 (Parameter Interception)
   - 参数验证 (ParameterValidationBehavior)
   - 参数转换和预处理
    ↓
2. 方法执行前处理 (Pre-Execution)
   - 执行前日志 (PreExecutionLoggingBehavior)
   - 权限检查、性能监控等
    ↓
3. 命令处理器执行 (Command Handler)
   - 业务逻辑处理
   - 返回原始结果
    ↓
4. 方法执行后处理 (Post-Execution)
   - 执行后日志 (PostExecutionLoggingBehavior)
   - 结果缓存、通知等
    ↓
5. 返回值贯穿处理 (Return Value Interception)
   - 返回值增强 (ReturnValueEnhancementBehavior)
   - 结果格式化和后处理
    ↓
最终结果输出 (TResult)

异常处理贯穿整个流程 (Exception Handling)
```

### 日志输出示例

```
[INFO] 🔍 参数验证开始: ProcessOrderCommand
[INFO] ✅ 参数验证通过: ProcessOrderCommand
[INFO] 🚀 方法执行开始: ProcessOrderCommand at 2024-01-15 10:30:00
[DEBUG] 📝 命令详情: {"product":"笔记本电脑","quantity":2,"priority":1}
[INFO] ✅ 方法执行完成: ProcessOrderCommand at 2024-01-15 10:30:01
[DEBUG] 📤 执行结果: "订单处理成功: 笔记本电脑 x2"
[INFO] 🔧 返回值增强处理: ProcessOrderCommand
[DEBUG] 📝 字符串结果增强: "订单处理成功: 笔记本电脑 x2" -> "[2024-01-15 10:30:01] 订单处理成功: 笔记本电脑 x2"
```

### 异常处理示例

```
[ERROR] ❌ 命令执行异常: ProcessOrderCommand, 异常类型: ArgumentException
[WARN] ⚠️ 参数异常，返回默认值
[INFO] 🔧 返回值增强处理: ProcessOrderCommand
[DEBUG] 📝 字符串结果增强: "处理失败" -> "[2024-01-15 10:30:01] 处理失败"
```

## 许可证

MIT License

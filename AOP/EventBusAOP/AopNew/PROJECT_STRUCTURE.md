# 📁 项目结构说明

## 🏗️ 整体架构

```
AopNew/
├── Common.Bus/                    # 核心命令总线库
│   ├── Core/                      # 核心接口和抽象
│   ├── Implementations/           # 具体实现类
│   ├── Monitoring/                # 监控相关
│   ├── Extensions/                # 扩展方法
│   └── Behaviors/                 # 管道行为（预留）
├── WebApp/                        # Web应用程序
│   ├── Controllers/               # API控制器
│   ├── Commands/                  # 命令定义
│   ├── Handlers/                  # 命令处理器
│   ├── Behaviors/                 # 管道行为实现
│   ├── Services/                  # 业务服务
│   └── Models/                    # 数据模型
└── 文档文件
```

## 📚 Common.Bus 库结构

### 🔧 Core/ - 核心接口
- `ICommand.cs` - 命令接口定义
- `ICommandBus.cs` - 命令总线接口
- `ICommandHandler.cs` - 命令处理器接口
- `ICommandPipelineBehavior.cs` - 管道行为接口
- `ICommandProcessor.cs` - 命令处理器接口
- `ICommandRequest.cs` - 命令请求接口

### ⚙️ Implementations/ - 具体实现
- `CommandBus.cs` - 标准命令总线实现
- `DataflowCommandBus.cs` - TPL Dataflow命令总线
- `BatchDataflowCommandBus.cs` - 批处理Dataflow命令总线
- `TypedDataflowCommandBus.cs` - 类型安全的Dataflow命令总线
- `MonitoredCommandBus.cs` - 带监控的命令总线包装器

### 📊 Monitoring/ - 监控系统
- `IDataflowMetrics.cs` - 数据流指标接口
- `IMetricsCollector.cs` - 指标收集器接口

### 🔌 Extensions/ - 扩展方法
- `ServiceCollectionExtensions.cs` - 依赖注入扩展方法

## 🌐 WebApp 应用结构

### 🎮 Controllers/ - API控制器
- `DataflowDemoController.cs` - 数据流演示控制器
- `MonitoringController.cs` - 监控控制器（SSE实时监控）
- `PerformanceTestController.cs` - 性能测试控制器
- `WeatherForecastController.cs` - 示例控制器

### 📝 Commands/ - 命令定义
- 存放具体的命令类（如 `CreateOrderCommand`, `ProcessOrderCommand`）

### 🎯 Handlers/ - 命令处理器
- 存放命令处理器的具体实现

### 🔄 Behaviors/ - 管道行为
- `LoggingBehavior.cs` - 日志记录行为
- `TransactionBehavior.cs` - 事务管理行为
- `ValidationBehavior.cs` - 验证行为

### 🏢 Services/ - 业务服务
- 存放业务逻辑服务类

### 📋 Models/ - 数据模型
- `WeatherForecast.cs` - 示例数据模型

## 🚀 使用方式

### 1. 核心命令总线
```csharp
// 注册服务
builder.Services.AddTypedDataflowCommandBus(maxConcurrency: Environment.ProcessorCount * 2);
builder.Services.AddMetricsCollector(TimeSpan.FromSeconds(1));

// 使用命令总线
var result = await _commandBus.SendAsync<CreateOrderCommand, string>(command);
```

### 2. 实时监控
```csharp
// 访问监控面板
GET /api/Monitoring/dashboard

// SSE实时数据流
GET /api/Monitoring/stream

// 获取当前指标
GET /api/Monitoring/metrics
```

### 3. 管道行为
```csharp
// 自动注册管道行为
builder.Services.AddScoped<ICommandPipelineBehavior<CreateOrderCommand, string>, LoggingBehavior<CreateOrderCommand, string>>();
builder.Services.AddScoped<ICommandPipelineBehavior<CreateOrderCommand, string>, ValidationBehavior<CreateOrderCommand, string>>();
```

## 📈 性能特性

- **并发处理**: 支持多线程并发处理命令
- **批处理**: 支持批量处理命令以提高吞吐量
- **背压控制**: 自动控制处理速度防止内存溢出
- **实时监控**: SSE实时推送性能指标
- **类型安全**: 强类型命令处理，减少运行时错误

## 🔧 扩展点

- **自定义命令**: 实现 `ICommand<TResult>` 接口
- **自定义处理器**: 实现 `ICommandHandler<TCommand, TResult>` 接口
- **自定义行为**: 实现 `ICommandPipelineBehavior<TCommand, TResult>` 接口
- **自定义监控**: 实现 `IMetricsCollector` 接口

# 基于时间参数的泛型打包请求优化

## 概述

通过引入时间戳作为请求的唯一标识和排序依据，我们成功简化了泛型打包请求的复杂性，同时提供了更好的性能和监控能力。

## 主要优化点

### 1. 简化泛型复杂性

**优化前：**
```csharp
// 需要复杂的泛型类型参数
public class CommandRequest<TCommand, TResult> : ICommandRequest
    where TCommand : ICommand<TResult>
{
    public TCommand Command { get; }
    public TaskCompletionSource<TResult> TaskCompletionSource { get; }
    // 复杂的类型转换和缓存机制
}
```

**优化后：**
```csharp
// 基于时间戳的统一请求类，无需泛型约束
public class TimeBasedCommandRequest
{
    public string RequestId { get; } // 基于时间戳的唯一ID
    public DateTime CreatedAt { get; }
    public Type CommandType { get; }
    public Type ResultType { get; }
    public object Command { get; }
    public TaskCompletionSource<object> TaskCompletionSource { get; }
}
```

### 2. 时间窗口批处理优化

**新增功能：**
- 基于时间窗口的自动批处理
- 相同时间窗口内的请求可以一起处理
- 支持超时机制和优先级排序
- 减少对象创建和垃圾回收压力

```csharp
public class TimeWindowManager
{
    private readonly TimeSpan _windowSize;
    private readonly ConcurrentDictionary<string, List<TimeBasedCommandRequest>> _pendingRequests;
    
    // 自动收集时间窗口内的请求进行批处理
    public void AddRequest(TimeBasedCommandRequest request)
    {
        var windowId = request.GetTimeWindowId(_windowSize);
        // 添加到对应的时间窗口
    }
}
```

### 3. 简化的缓存机制

**优化前：**
```csharp
// 需要为每个命令类型维护单独的缓存
private readonly ConcurrentDictionary<Type, object> _handlerCache = new();
private readonly ConcurrentDictionary<Type, object[]> _behaviorsCache = new();
private readonly ConcurrentDictionary<Type, Func<object, object, CancellationToken, Task<object>>> _pipelineCache = new();
```

**优化后：**
```csharp
// 统一的缓存机制，基于时间戳管理
private readonly ConcurrentDictionary<Type, Func<object>> _handlerCache = new();
private readonly ConcurrentDictionary<Type, Func<object[]>> _behaviorsCache = new();
// 支持基于时间的缓存清理
```

### 4. 增强的监控和调试能力

**新增监控指标：**
- 时间窗口统计信息
- 请求处理时间分析
- 批处理效率指标
- 基于时间戳的请求追踪

```csharp
public class TimeBasedCommandBusMetrics
{
    public long ProcessedCommands { get; set; }
    public long ProcessedBatches { get; set; }
    public TimeSpan AverageProcessingTime { get; set; }
    public TimeWindowMetrics TimeWindowMetrics { get; set; }
    public double Throughput { get; set; }
    public double AverageBatchSize { get; set; }
}
```

## 性能提升

### 1. 批处理优化
- **吞吐量提升**：相同时间窗口内的请求可以并行处理
- **延迟优化**：减少单个请求的处理延迟
- **资源利用**：更好的CPU和内存利用率

### 2. 内存优化
- **对象复用**：减少TaskCompletionSource的创建
- **缓存效率**：基于时间的缓存管理
- **垃圾回收**：减少GC压力

### 3. 监控优化
- **实时追踪**：基于时间戳的请求追踪
- **性能分析**：时间窗口级别的性能分析
- **故障诊断**：更精确的错误定位

## 使用示例

### 1. 基本使用
```csharp
// 注册服务
builder.Services.AddTimeBasedCommandBus(
    maxConcurrency: Environment.ProcessorCount * 2,
    enableBatchProcessing: true,
    batchWindowSize: TimeSpan.FromMilliseconds(50)
);

// 使用命令
var result = await _commandBus.SendAsync<TimeBasedOrderCommand, TimeBasedOrderResult>(command);
```

### 2. 批处理测试
```csharp
[HttpPost("batch-window")]
public async Task<IActionResult> ProcessBatchWindow([FromBody] BatchWindowTestRequest request)
{
    var tasks = new List<Task<TimeBasedOrderResult>>();
    
    // 创建多个命令，它们会在相同的时间窗口内被批处理
    for (int i = 0; i < request.Count; i++)
    {
        var command = new TimeBasedOrderCommand($"Product-{i}", request.Quantity, request.Priority);
        tasks.Add(_commandBus.SendAsync<TimeBasedOrderCommand, TimeBasedOrderResult>(command));
    }
    
    var results = await Task.WhenAll(tasks);
    return Ok(results);
}
```

### 3. 性能监控
```csharp
[HttpGet("metrics")]
public IActionResult GetTimeBasedMetrics()
{
    if (_commandBus is TimeBasedCommandBus timeBasedBus)
    {
        var metrics = timeBasedBus.GetMetrics();
        return Ok(new
        {
            ProcessedCommands = metrics.ProcessedCommands,
            ProcessedBatches = metrics.ProcessedBatches,
            Throughput = metrics.Throughput,
            AverageBatchSize = metrics.AverageBatchSize,
            TimeWindowMetrics = metrics.TimeWindowMetrics
        });
    }
    return Ok(new { Message = "Time-based CommandBus not available" });
}
```

## 配置选项

### 1. 时间窗口大小
```csharp
// 50毫秒时间窗口，适合高频率请求
batchWindowSize: TimeSpan.FromMilliseconds(50)

// 100毫秒时间窗口，适合中等频率请求
batchWindowSize: TimeSpan.FromMilliseconds(100)

// 200毫秒时间窗口，适合低频率请求
batchWindowSize: TimeSpan.FromMilliseconds(200)
```

### 2. 并发控制
```csharp
// 基于CPU核心数的并发控制
maxConcurrency: Environment.ProcessorCount * 2

// 固定并发数
maxConcurrency: 16

// 高并发场景
maxConcurrency: 32
```

### 3. 批处理开关
```csharp
// 启用批处理（推荐）
enableBatchProcessing: true

// 禁用批处理，直接处理
enableBatchProcessing: false
```

## 迁移指南

### 从原有CommandBus迁移

1. **更新服务注册**：
```csharp
// 原来
builder.Services.AddTypedDataflowCommandBus();

// 现在
builder.Services.AddTimeBasedCommandBus();
```

2. **更新命令定义**：
```csharp
// 原来
public record ProcessOrderCommand(string Product, int Quantity) : ICommand<string>;

// 现在（可选，保持兼容）
public record TimeBasedOrderCommand(string Product, int Quantity, int Priority = 1) : ICommand<TimeBasedOrderResult>;
```

3. **更新处理器**：
```csharp
// 原来
public class ProcessOrderHandler : ICommandHandler<ProcessOrderCommand, string>

// 现在
public class TimeBasedOrderHandler : ICommandHandler<TimeBasedOrderCommand, TimeBasedOrderResult>
```

## 总结

通过引入时间戳作为请求的唯一标识和排序依据，我们成功：

1. **简化了泛型打包的复杂性**：移除了复杂的泛型约束和类型转换
2. **提升了性能**：通过时间窗口批处理提高了吞吐量
3. **增强了监控能力**：基于时间戳的请求追踪和性能分析
4. **保持了兼容性**：现有代码可以无缝迁移

这个优化方案特别适合高并发、高吞吐量的场景，同时提供了更好的可观测性和调试能力。

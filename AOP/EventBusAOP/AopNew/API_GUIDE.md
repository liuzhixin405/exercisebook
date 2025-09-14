# CommandBus API 使用指南

## 概述

本指南详细介绍了如何使用CommandBus项目的各种API端点，包括如何通过枚举选择不同的CommandBus实现。

## 基础概念

### CommandBus类型枚举

```csharp
public enum CommandBusType
{
    Standard,        // 标准CommandBus - 同步处理
    Dataflow,        // TPL Dataflow CommandBus - 异步并发处理
    BatchDataflow,   // 批处理Dataflow CommandBus - 批量处理
    TypedDataflow,   // 类型安全Dataflow CommandBus - 强类型
    Monitored        // 带监控的CommandBus - 性能监控
}
```

## 统一演示API

### 1. 处理订单

**端点**: `POST /api/CommandBusDemo/process-order/{busType}`

**参数**:
- `busType`: CommandBus类型（Standard, Dataflow, BatchDataflow, TypedDataflow, Monitored）
- `command`: 订单命令对象

**请求体**:
```json
{
    "product": "笔记本电脑",
    "quantity": 2,
    "priority": 1
}
```

**响应**:
```json
{
    "success": true,
    "result": "Order processed: 笔记本电脑 x 2 (Priority: 1) - Processing time: 45ms",
    "busType": "Dataflow",
    "message": "使用 Dataflow 处理订单成功"
}
```

**示例**:
```bash
curl -X POST "https://localhost:5056/api/CommandBusDemo/process-order/Dataflow" \
  -H "Content-Type: application/json" \
  -d '{
    "product": "笔记本电脑",
    "quantity": 2,
    "priority": 1
  }'
```

### 2. 创建用户

**端点**: `POST /api/CommandBusDemo/create-user/{busType}`

**请求体**:
```json
{
    "name": "张三",
    "email": "zhangsan@example.com",
    "age": 25
}
```

**响应**:
```json
{
    "success": true,
    "userId": 1234,
    "busType": "TypedDataflow",
    "message": "使用 TypedDataflow 创建用户成功"
}
```

### 3. 发送邮件

**端点**: `POST /api/CommandBusDemo/send-email/{busType}`

**请求体**:
```json
{
    "to": "user@example.com",
    "subject": "测试邮件",
    "body": "这是一封测试邮件"
}
```

**响应**:
```json
{
    "success": true,
    "emailSent": true,
    "busType": "Monitored",
    "message": "使用 Monitored 发送邮件成功"
}
```

### 4. 并发处理订单

**端点**: `POST /api/CommandBusDemo/concurrent-process-orders/{busType}`

**请求体**:
```json
[
    {
        "product": "手机",
        "quantity": 1,
        "priority": 2
    },
    {
        "product": "平板电脑",
        "quantity": 1,
        "priority": 1
    },
    {
        "product": "耳机",
        "quantity": 2,
        "priority": 3
    }
]
```

**响应**:
```json
{
    "success": true,
    "results": [
        "Order processed: 手机 x 1 (Priority: 2) - Processing time: 23ms",
        "Order processed: 平板电脑 x 1 (Priority: 1) - Processing time: 67ms",
        "Order processed: 耳机 x 2 (Priority: 3) - Processing time: 34ms"
    ],
    "count": 3,
    "busType": "BatchDataflow",
    "message": "使用 BatchDataflow 并发处理 3 个订单成功"
}
```

### 5. 获取指标

**端点**: `GET /api/CommandBusDemo/metrics/{busType}`

**响应**:
```json
{
    "success": true,
    "metrics": {
        "processedCommands": 150,
        "failedCommands": 2,
        "totalProcessingTime": "00:02:30.500",
        "averageProcessingTime": "00:00:01.003",
        "successRate": 0.9867,
        "availableConcurrency": 6,
        "maxConcurrency": 8,
        "inputQueueSize": 0
    },
    "busType": "Dataflow",
    "message": "获取 Dataflow 指标成功"
}
```

### 6. 获取可用类型

**端点**: `GET /api/CommandBusDemo/available-types`

**响应**:
```json
{
    "success": true,
    "availableTypes": [
        {
            "value": "Standard",
            "description": "标准CommandBus - 同步处理，适合简单场景"
        },
        {
            "value": "Dataflow",
            "description": "TPL Dataflow CommandBus - 异步并发处理，适合高并发场景"
        },
        {
            "value": "BatchDataflow",
            "description": "批处理Dataflow CommandBus - 批量处理，适合大批量数据场景"
        },
        {
            "value": "TypedDataflow",
            "description": "类型安全Dataflow CommandBus - 强类型，适合复杂业务场景"
        },
        {
            "value": "Monitored",
            "description": "带监控的CommandBus - 包含性能监控，适合生产环境"
        }
    ],
    "count": 5
}
```

## 专用控制器API

每个CommandBus实现都有专门的控制器，提供相同的功能但针对特定实现优化。

### Standard CommandBus

**基础URL**: `/api/StandardCommandBus`

- `POST /process-order` - 处理订单
- `POST /create-user` - 创建用户
- `POST /send-email` - 发送邮件
- `POST /batch-process-orders` - 批量处理订单

### Dataflow CommandBus

**基础URL**: `/api/DataflowCommandBus`

- `POST /process-order` - 处理订单
- `POST /create-user` - 创建用户
- `POST /send-email` - 发送邮件
- `POST /concurrent-process-orders` - 并发处理订单
- `GET /metrics` - 获取指标

### Batch Dataflow CommandBus

**基础URL**: `/api/BatchDataflowCommandBus`

- `POST /process-order` - 处理订单
- `POST /create-user` - 创建用户
- `POST /send-email` - 发送邮件
- `POST /batch-process-orders` - 批量处理订单
- `GET /metrics` - 获取指标

### Typed Dataflow CommandBus

**基础URL**: `/api/TypedDataflowCommandBus`

- `POST /process-order` - 处理订单
- `POST /create-user` - 创建用户
- `POST /send-email` - 发送邮件
- `POST /concurrent-process-orders` - 并发处理订单
- `GET /metrics` - 获取指标

### Monitored CommandBus

**基础URL**: `/api/MonitoredCommandBus`

- `POST /process-order` - 处理订单
- `POST /create-user` - 创建用户
- `POST /send-email` - 发送邮件
- `POST /concurrent-process-orders` - 并发处理订单
- `GET /metrics` - 获取指标

## 监控API

### 1. 监控面板

**端点**: `GET /api/Monitoring/dashboard`

返回HTML监控面板页面，包含实时图表和指标显示。

### 2. SSE实时数据流

**端点**: `GET /api/Monitoring/stream`

返回Server-Sent Events数据流，实时推送性能指标。

**响应格式**:
```
data: {"timestamp":"2024-01-15T10:30:00Z","metrics":{"processedCommands":150,"failedCommands":2,"successRate":0.9867}}

data: {"timestamp":"2024-01-15T10:30:01Z","metrics":{"processedCommands":152,"failedCommands":2,"successRate":0.9868}}
```

### 3. 获取当前指标

**端点**: `GET /api/Monitoring/metrics`

**响应**:
```json
{
    "success": true,
    "metrics": {
        "processedCommands": 150,
        "failedCommands": 2,
        "totalProcessingTime": "00:02:30.500",
        "averageProcessingTime": "00:00:01.003",
        "successRate": 0.9867,
        "availableConcurrency": 6,
        "maxConcurrency": 8,
        "inputQueueSize": 0
    },
    "timestamp": "2024-01-15T10:30:00Z"
}
```

## 错误处理

所有API都返回统一的错误格式：

```json
{
    "success": false,
    "error": "错误描述信息",
    "busType": "Dataflow"
}
```

常见错误：

- **400 Bad Request**: 请求参数错误
- **500 Internal Server Error**: 服务器内部错误
- **ArgumentException**: 不支持的CommandBus类型

## 性能测试

### 使用curl进行性能测试

```bash
# 测试标准CommandBus性能
for i in {1..100}; do
  curl -X POST "https://localhost:5056/api/CommandBusDemo/process-order/Standard" \
    -H "Content-Type: application/json" \
    -d '{"product":"测试产品","quantity":1,"priority":1}' &
done
wait

# 测试Dataflow CommandBus性能
for i in {1..100}; do
  curl -X POST "https://localhost:5056/api/CommandBusDemo/process-order/Dataflow" \
    -H "Content-Type: application/json" \
    -d '{"product":"测试产品","quantity":1,"priority":1}' &
done
wait
```

### 使用Postman进行测试

1. 导入API集合
2. 设置环境变量 `baseUrl = https://localhost:5056`
3. 运行性能测试脚本

## 最佳实践

### 1. 选择合适的CommandBus类型

- **Standard**: 简单业务逻辑，低并发场景
- **Dataflow**: 高并发场景，需要异步处理
- **BatchDataflow**: 批量数据处理，提高吞吐量
- **TypedDataflow**: 复杂业务逻辑，需要类型安全
- **Monitored**: 生产环境，需要性能监控

### 2. 错误处理

```csharp
try
{
    var result = await commandBus.SendAsync<MyCommand, string>(command);
    return Ok(result);
}
catch (Exception ex)
{
    _logger.LogError(ex, "Command processing failed");
    return BadRequest(new { error = ex.Message });
}
```

### 3. 监控和日志

- 使用`MonitoredCommandBus`获取性能指标
- 启用SSE实时监控
- 配置适当的日志级别

### 4. 并发控制

- 根据系统资源调整并发数
- 使用批处理提高吞吐量
- 监控队列大小避免内存溢出

## 故障排除

### 常见问题

1. **CommandBus类型不存在**
   - 检查枚举值是否正确
   - 确认服务已正确注册

2. **命令处理器未找到**
   - 检查命令和处理器是否已注册
   - 确认命名空间和类型名称正确

3. **性能问题**
   - 调整并发数设置
   - 使用批处理模式
   - 检查系统资源使用情况

4. **监控数据不准确**
   - 确认监控服务已启动
   - 检查SSE连接是否正常
   - 验证指标收集器配置

### 调试技巧

1. 启用详细日志
2. 使用监控面板观察实时指标
3. 检查依赖注入配置
4. 验证命令和处理器注册

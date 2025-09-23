# gRPC-Web 实时数据推送系统

一个基于 gRPC-Web 的实时数据推送系统，包含 .NET Core 服务端和 Web 浏览器客户端。

## 功能特性

- ✅ **实时数据推送**：服务端向客户端持续推送实时数据
- ✅ **gRPC-Web 协议**：使用 gRPC-Web 协议实现浏览器兼容
- ✅ **流式传输**：支持服务端流式数据传输
- ✅ **自动重连**：客户端具备自动重连机制
- ✅ **错误处理**：完善的错误处理和日志记录

## 项目结构

```
├── GrpcRealtimePush/           # .NET Core 服务端
│   ├── Services/
│   │   └── ChatService.cs      # gRPC 服务实现
│   ├── Protos/
│   │   └── chat.proto          # Protocol Buffers 定义
│   └── Program.cs              # 服务端启动配置
├── client/                     # Web 客户端
│   ├── generated/              # 生成的 gRPC 客户端代码
│   │   ├── chat_pb_browser.js           # Protocol Buffers 消息类
│   │   └── chat_grpc_web_pb_browser.js  # gRPC 服务客户端
│   ├── grpc-web-shim.js       # gRPC-Web 协议兼容层
│   ├── client.js              # 主要业务逻辑
│   ├── index.html             # 用户界面
│   └── 使用说明.txt            # 详细使用说明
└── generate-client.ps1         # 客户端代码生成脚本（从 proto 生成 JS 代码）
```

### 前端文件说明

#### 核心文件
- **index.html**: 用户界面，包含HTML结构、CSS样式和脚本引用
- **client.js**: 主要业务逻辑，包含 `RealtimePushClient` 类和所有交互功能
- **grpc-web-shim.js**: gRPC-Web协议兼容层，处理浏览器与gRPC服务的底层通信

#### 生成的代码文件
- **chat_pb_browser.js**: Protocol Buffers 消息类定义，包含数据序列化/反序列化
- **chat_grpc_web_pb_browser.js**: gRPC 服务客户端代码，提供服务方法调用接口

## 技术栈

### 服务端
- .NET Core 9.0
- Grpc.AspNetCore
- Grpc.AspNetCore.Web

### 客户端
- HTML5 + JavaScript ES6+
- gRPC-Web Protocol
- Google Protocol Buffers (protobuf)

## 快速开始

### 1. 启动服务端

```bash
cd GrpcRealtimePush
dotnet run
```

服务端将在以下地址启动：
- HTTP: `http://localhost:5200`
- HTTPS: `https://localhost:5201`

### 2. 打开客户端

在浏览器中打开 `client/index.html` 文件，或者使用本地 HTTP 服务器：

```bash
# 使用 Python 启动本地服务器
cd client
python -m http.server 8080

# 然后访问 http://localhost:8080
```

### 3. 测试实时推送

1. 点击 "🚀 启动实时推送" 按钮
2. 观察实时数据流
3. 点击 "⏹️ 停止推送" 停止数据流

## API 定义

### Protocol Buffers 定义

```protobuf
syntax = "proto3";

package chat;

service ChatService {
  rpc StartRealtimePush(RealtimePushRequest) returns (stream RealtimePushResponse);
}

message RealtimePushRequest {
  string client_id = 1;
  int64 timestamp = 2;
}

message RealtimePushResponse {
  string data = 1;
  int64 timestamp = 2;
  string data_type = 3;
}
```

### 服务端 API

- **StartRealtimePush**: 启动实时数据推送流
  - 输入: `RealtimePushRequest`
  - 输出: `stream RealtimePushResponse`
  - 功能: 持续推送模拟的实时数据

## 开发说明

### 代码生成流程

#### 1. Protocol Buffers 定义
首先在 `GrpcRealtimePush/Protos/chat.proto` 中定义服务和消息结构：

```protobuf
syntax = "proto3";
package chat;

service ChatService {
  rpc StartRealtimePush(RealtimePushRequest) returns (stream RealtimePushResponse);
}

message RealtimePushRequest {
  string client_id = 1;
  int64 timestamp = 2;
}

message RealtimePushResponse {
  string data = 1;
  int64 timestamp = 2;
  string data_type = 3;
}
```

#### 2. 服务端代码生成
.NET Core 项目会自动根据 `.proto` 文件生成 C# 代码：
- 构建时自动生成：`dotnet build`
- 生成的代码位于 `obj/` 目录中
- 包含消息类和服务基类

#### 3. 客户端代码生成
使用 `generate-client.ps1` 脚本生成浏览器兼容的 JavaScript 代码：

```powershell
# 生成命令
protoc -I=GrpcRealtimePush\Protos \
  --js_out=import_style=commonjs:client\generated \
  --grpc-web_out=import_style=commonjs,mode=grpcwebtext:client\generated \
  GrpcRealtimePush\Protos\chat.proto
```

生成的文件：
- `chat_pb.js` - Protocol Buffers 消息类（CommonJS 格式）
- `chat_grpc_web_pb.js` - gRPC-Web 客户端（CommonJS 格式）

#### 4. 浏览器适配
由于生成的代码是 CommonJS 格式，需要手动适配为浏览器兼容格式：
- `chat_pb_browser.js` - 适配后的消息类
- `chat_grpc_web_pb_browser.js` - 适配后的客户端

### 前端开发接入指南

#### 步骤1：获取 Proto 文件
从后端开发获取 `.proto` 文件，了解：
- 服务名称和方法
- 请求和响应消息结构
- 数据类型定义

#### 步骤2：生成客户端代码
1. 安装 Protocol Buffers 编译器 (`protoc`)
2. 安装 gRPC-Web 插件
3. 运行生成脚本：`.\generate-client.ps1`

#### 步骤3：适配浏览器环境
由于生成的代码是 Node.js 格式，需要手动适配：

```javascript
// 示例：适配消息类
window.proto.chat.RealtimePushRequest = function(opt_data) {
    jspb.Message.initialize(this, opt_data, 0, -1, null, null);
};

// 添加 getter/setter 方法
window.proto.chat.RealtimePushRequest.prototype.getClientId = function() {
    return jspb.Message.getFieldWithDefault(this, 1, "");
};
```

#### 步骤4：实现客户端逻辑
```javascript
// 创建客户端
const client = new proto.chat.ChatServiceClient('https://localhost:5201');

// 创建请求
const request = new proto.chat.RealtimePushRequest();
request.setClientId('web-client');
request.setTimestamp(Date.now());

// 启动流式推送
const stream = client.startRealtimePush(request, {});

// 处理数据
stream.on('data', (response) => {
    console.log('收到数据:', response.getData());
});
```

### 修改和扩展

#### 修改 Protocol Buffers
1. 编辑 `GrpcRealtimePush/Protos/chat.proto`
2. 重新构建服务端: `dotnet build`
3. 重新生成客户端代码: `.\generate-client.ps1`
   - 这个脚本会从 proto 文件自动生成 JavaScript 客户端代码
   - 生成的代码需要手动适配浏览器环境
4. 手动更新浏览器适配代码

#### 自定义数据推送
修改 `GrpcRealtimePush/Services/ChatService.cs` 中的 `StartRealtimePush` 方法：

```csharp
public override async Task StartRealtimePush(RealtimePushRequest request, 
    IServerStreamWriter<RealtimePushResponse> responseStream, ServerCallContext context)
{
    // 自定义推送逻辑
    while (!context.CancellationToken.IsCancellationRequested)
    {
        var response = new RealtimePushResponse
        {
            Data = "自定义数据",
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            DataType = "自定义类型"
        };
        
        await responseStream.WriteAsync(response);
        await Task.Delay(1000, context.CancellationToken);
    }
}
```

#### 客户端自定义
修改 `client/client.js` 来自定义：
- 数据显示格式
- 重连逻辑
- 错误处理
- UI交互

## 故障排除

### 常见问题

1. **CORS 错误**
   - 确保服务端已配置 CORS 策略
   - 使用 HTTPS 访问客户端

2. **连接失败**
   - 检查服务端是否正常启动
   - 确认端口 5201 未被占用

3. **数据解析错误**
   - 检查客户端生成的代码是否与服务端 proto 定义一致
   - 重新生成客户端代码

### 调试模式

打开浏览器开发者工具查看详细的调试信息和错误日志。

## 性能优化

- 服务端默认推送 100 条消息后自动停止
- 客户端自动清理超过 100 条的历史消息
- 使用 gRPC-Web 文本模式以获得更好的浏览器兼容性

## 许可证

MIT License

## 贡献

欢迎提交 Issue 和 Pull Request！
# MiniHttpApi - Gin 风格的 .NET HTTP 框架

基于 `HttpListener` 的轻量级 HTTP 框架，借鉴 Go Gin 的优雅 API 风格，采用面向对象设计。

## 项目结构

```
app/
├── MiniGin/                    # 类库项目
│   ├── MiniGin.csproj          # 类库项目文件
│   ├── Interfaces.cs           # 核心接口和委托
│   ├── Context.cs              # 请求上下文
│   ├── RouterGroup.cs          # 路由分组
│   ├── Engine.cs               # HTTP 引擎
│   ├── Route.cs                # 路由定义
│   ├── Middleware.cs           # 内置中间件
│   └── Gin.cs                  # 工厂方法
├── MiniHttpApi.csproj          # 示例应用
├── Program.cs                  # 示例代码
└── README.md
```

## 核心特性

- **面向对象设计**：`Engine`、`RouterGroup`、`Context` 清晰分层
- **链式路由**：支持 `GET/POST/PUT/DELETE/PATCH/HEAD/OPTIONS` 等 HTTP 方法
- **路由分组**：`app.Group("/api")` 支持前缀和中间件继承
- **路径参数**：`:id` 风格的动态路由参数
- **通配符路由**：`*path` 捕获剩余路径
- **中间件管道**：全局/分组级别的中间件支持
- **丰富的 Context API**：参数绑定、JSON 响应、快捷方法
- **Swagger 集成**：自动生成 OpenAPI 文档

## 快速开始

### 1. 添加项目引用

```xml
<ItemGroup>
  <ProjectReference Include="MiniGin\MiniGin.csproj" />
</ItemGroup>

<!-- 排除类库源文件（如果 MiniGin 是子文件夹） -->
<ItemGroup>
  <Compile Remove="MiniGin\**\*.cs" />
  <None Remove="MiniGin\**\*" />
</ItemGroup>
```

### 2. 创建应用

```csharp
using MiniGin;

// 创建引擎（带默认中间件）
var app = Gin.Default();

// 或创建空引擎
// var app = Gin.New();

// 启用 Swagger
app.UseSwagger("My API", "v1");

// 定义路由
app.GET("/", async ctx => await ctx.String(200, "Hello World!"));
app.GET("/ping", async ctx => await ctx.JSON(new { message = "pong" }));

// 启动服务器
await app.Run("http://localhost:5000/");
```

## API 参考

### 路由定义

```csharp
// 基本路由
app.GET("/users", async ctx => { ... });
app.POST("/users", async ctx => { ... });
app.PUT("/users/:id", async ctx => { ... });
app.DELETE("/users/:id", async ctx => { ... });
app.PATCH("/users/:id", async ctx => { ... });

// 通用方法
app.Handle("GET", "/custom", async ctx => { ... });

// 多处理器链
app.GET("/protected", authMiddleware, async ctx => { ... });
```

### 路由分组

```csharp
var api = app.Group("/api");
api.Use(myMiddleware);

// 嵌套分组
var admin = api.Group("/admin");
admin.Use(Middleware.BasicAuth((u, p) => u == "admin" && p == "secret"));

admin.GET("/dashboard", async ctx => {
    await ctx.JSON(new { message = "Admin Dashboard" });
});
```

### Context API

```csharp
// 获取路径参数
var id = ctx.Param("id");

// 获取查询参数
var page = ctx.Query<int>("page") ?? 1;
var name = ctx.Query("name"); // string?

// 绑定 JSON 请求体
var user = await ctx.BindAsync<CreateUserRequest>();

// 获取原始请求体
var body = await ctx.GetRawDataAsync();

// 设置响应头
ctx.Header("X-Custom-Header", "value");

// JSON 响应
await ctx.JSON(new { message = "success" });

// 文本响应
await ctx.String(200, "Hello World");

// 状态码快捷方法
await ctx.OK(data);           // 200
await ctx.Created(data);      // 201
await ctx.NoContent();        // 204
await ctx.BadRequest(error);  // 400
await ctx.NotFound();         // 404
await ctx.InternalServerError(error); // 500

// 存取上下文数据
ctx.Set("user", currentUser);
var user = ctx.Get<User>("user");

// 中止后续处理器
ctx.Abort();
ctx.AbortWithStatus(403);
await ctx.AbortWithError(403, "Forbidden");
```

### 内置中间件

```csharp
// Logger - 请求日志
app.Use(Middleware.Logger());

// Recovery - 异常恢复
app.Use(Middleware.Recovery());

// CORS - 跨域支持
app.Use(Middleware.CORS());
app.Use(Middleware.CORS(new CorsConfig {
    AllowOrigins = new[] { "https://example.com" },
    AllowMethods = new[] { "GET", "POST" },
    AllowHeaders = new[] { "Authorization" }
}));

// BasicAuth - HTTP 基本认证
app.Use(Middleware.BasicAuth((username, password) => 
    username == "admin" && password == "secret"));

// ApiKey - API 密钥认证
app.Use(Middleware.ApiKey("X-Api-Key", key => key == "my-secret-key"));

// Static - 静态文件服务
app.Use(Middleware.Static("/static", "./wwwroot"));

// RequestId - 请求 ID
app.Use(Middleware.RequestId());

// Timeout - 请求超时
app.Use(Middleware.Timeout(TimeSpan.FromSeconds(30)));
```

### 自定义中间件

```csharp
// 函数式中间件
app.Use(async ctx => {
    Console.WriteLine($"Before: {ctx.Request.Url}");
    // 继续执行后续处理器（通过不调用 Abort）
});

// 实现 IMiddleware 接口
public class MyMiddleware : IMiddleware
{
    public HandlerFunc Handler => async ctx => {
        // 前置逻辑
        ctx.Set("start_time", DateTime.Now);
        
        // 后续处理器会自动执行
        // 如果需要中止，调用 ctx.Abort()
    };
}

app.Use(new MyMiddleware());
```

### Swagger

```csharp
// 启用 Swagger
app.UseSwagger("API Title", "v1");

// 访问地址
// Swagger UI: http://localhost:5000/swagger
// OpenAPI JSON: http://localhost:5000/swagger/v1/swagger.json
```

## 完整示例

```csharp
using MiniGin;

var app = Gin.Default();
app.UseSwagger("User API", "v1");

// 全局中间件
app.Use(Middleware.CORS(), Middleware.RequestId());

// 根路由
app.GET("/", async ctx => await ctx.String(200, "API is ready!"));

// API 分组
var api = app.Group("/api");

api.GET("/users", async ctx => {
    var page = ctx.Query<int>("page") ?? 1;
    await ctx.JSON(new { users = new[] { "Alice", "Bob" }, page });
});

api.GET("/users/:id", async ctx => {
    var id = ctx.Param("id");
    await ctx.JSON(new { id, name = $"User_{id}" });
});

api.POST("/users", async ctx => {
    var user = await ctx.BindAsync<CreateUserRequest>();
    if (user == null) {
        await ctx.BadRequest(new { error = "Invalid request" });
        return;
    }
    await ctx.Created(new { id = 1, name = user.Name });
});

// 需要认证的管理路由
var admin = api.Group("/admin");
admin.Use(Middleware.BasicAuth((u, p) => u == "admin" && p == "123456"));

admin.GET("/dashboard", async ctx => {
    var user = ctx.Get<string>("user");
    await ctx.JSON(new { message = $"Welcome {user}!" });
});

await app.Run("http://localhost:5000/");

record CreateUserRequest(string Name, string Email);
```

## 运行

```powershell
dotnet run --project MiniHttpApi.csproj
```

- API 地址：`http://localhost:5000/`
- Swagger UI：`http://localhost:5000/swagger`

## 打包为 NuGet

```powershell
cd MiniGin
dotnet pack -c Release
```

生成的 `.nupkg` 文件位于 `MiniGin/bin/Release/`。

## 许可证

MIT

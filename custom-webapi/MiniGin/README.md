# MiniGin

一个轻量级的 Gin 风格 HTTP 框架，基于 .NET HttpListener 实现。

## 安装

```bash
dotnet add package MiniGin
```

或者项目引用：

```xml
<ProjectReference Include="..\MiniGin\MiniGin.csproj" />
```

## 快速开始

```csharp
using MiniGin;

// 方式 1：使用默认引擎（包含 Logger 和 Recovery）
var app = Gin.Default();

// 方式 2：使用空白引擎
var app = Gin.New();

// 方式 3：直接创建
var app = new Engine();

// 启用 Swagger
app.UseSwagger("My API", "v1");

// 注册路由
app.GET("/", async ctx => await ctx.String(200, "Hello World!"));
app.GET("/ping", async ctx => await ctx.JSON(new { message = "pong" }));

// 启动服务器
await app.Run("http://localhost:5000/");
```

## 路由

### 基本路由

```csharp
app.GET("/users", handler);
app.POST("/users", handler);
app.PUT("/users/:id", handler);
app.DELETE("/users/:id", handler);
app.PATCH("/users/:id", handler);
```

### 路由参数

```csharp
// :id 是路由参数
app.GET("/users/:id", async ctx =>
{
    var id = ctx.Param("id");
    await ctx.JSON(new { id });
});

// *path 是通配符参数
app.GET("/files/*path", async ctx =>
{
    var path = ctx.Param("path");
    await ctx.JSON(new { path });
});
```

### 路由分组

```csharp
var api = app.Group("/api");
api.GET("/users", handler);     // GET /api/users
api.POST("/users", handler);    // POST /api/users

var v1 = api.Group("/v1");
v1.GET("/items", handler);      // GET /api/v1/items
```

## 中间件

### 使用中间件

```csharp
// 全局中间件
app.Use(Middleware.Logger());
app.Use(Middleware.CORS());

// 分组中间件
var api = app.Group("/api");
api.Use(Middleware.ApiKey("X-API-Key", key => key == "secret"));
```

### 内置中间件

| 中间件 | 说明 |
|--------|------|
| `Middleware.Logger()` | 请求日志 |
| `Middleware.Timer()` | 请求计时 |
| `Middleware.Recovery()` | 错误恢复 |
| `Middleware.CORS()` | 跨域支持 |
| `Middleware.BasicAuth()` | HTTP Basic 认证 |
| `Middleware.ApiKey()` | API Key 认证 |
| `Middleware.RequestId()` | 请求 ID |
| `Middleware.Headers()` | 自定义响应头 |
| `Middleware.Static()` | 静态文件服务 |

### 自定义中间件

```csharp
// 函数式
app.Use(async ctx =>
{
    Console.WriteLine("Before");
    // next 通过不调用 ctx.Abort() 来继续
});

// 类式
public class MyMiddleware : IMiddleware
{
    public async Task InvokeAsync(Context ctx, HandlerFunc next)
    {
        Console.WriteLine("Before");
        await next(ctx);
        Console.WriteLine("After");
    }
}

app.Use(new MyMiddleware());
```

## Context API

### 请求参数

```csharp
ctx.Param("id")              // 路由参数
ctx.Param("id", "default")   // 带默认值

ctx.Query("page")            // 查询参数
ctx.Query("page", "1")       // 带默认值
ctx.Query<int>("page")       // 类型转换

ctx.GetHeader("Authorization")
ctx.GetHeader("X-Custom", "default")
```

### 请求体绑定

```csharp
var user = await ctx.BindAsync<User>();      // 可能为 null
var user = await ctx.BindAsync(new User());  // 带默认值
var user = await ctx.MustBindAsync<User>();  // 失败抛异常
var body = await ctx.GetRawBodyAsync();      // 原始字符串
```

### 响应方法

```csharp
await ctx.JSON(200, data);       // JSON 响应
await ctx.JSON(data);            // JSON (200)
await ctx.String(200, "text");   // 纯文本
await ctx.HTML(200, "<h1>Hi</h1>");
await ctx.Status(204);           // 仅状态码
await ctx.Data(200, "image/png", bytes);
await ctx.Redirect("/new-url");
```

### 快捷响应

```csharp
await ctx.OK(data);           // 200
await ctx.Created(data);      // 201
await ctx.NoContent();        // 204
await ctx.BadRequest(error);  // 400
await ctx.Unauthorized();     // 401
await ctx.Forbidden();        // 403
await ctx.NotFound();         // 404
await ctx.InternalServerError(); // 500
```

### 上下文数据

```csharp
ctx.Set("user", user);
var user = ctx.Get<User>("user");
var exists = ctx.Has("user");
```

### 流程控制

```csharp
ctx.Abort();                        // 中止后续处理
await ctx.AbortWithStatus(401);     // 中止并返回状态码
await ctx.AbortWithJSON(401, err);  // 中止并返回 JSON
```

## Swagger

```csharp
app.UseSwagger("My API", "v1");
await app.Run("http://localhost:5000/");

// 访问 http://localhost:5000/swagger
```

## 完整示例

```csharp
using MiniGin;

var app = Gin.Default();
app.UseSwagger("User API", "v1");

app.Use(Middleware.CORS(), Middleware.RequestId());

app.GET("/", async ctx => await ctx.String(200, "Welcome!"));

var api = app.Group("/api");

api.GET("/users", async ctx =>
{
    var page = ctx.Query<int>("page") ?? 1;
    await ctx.JSON(new { users = new[] { "Alice", "Bob" }, page });
});

api.GET("/users/:id", async ctx =>
{
    var id = ctx.Param("id");
    await ctx.JSON(new { id, name = $"User_{id}" });
});

api.POST("/users", async ctx =>
{
    var user = await ctx.BindAsync<CreateUserRequest>();
    if (user == null)
    {
        await ctx.BadRequest(new { error = "Invalid body" });
        return;
    }
    await ctx.Created(new { id = 1, name = user.Name });
});

var admin = api.Group("/admin");
admin.Use(Middleware.BasicAuth((u, p) => u == "admin" && p == "123456"));
admin.GET("/dashboard", async ctx =>
{
    var user = ctx.Get<string>("user");
    await ctx.JSON(new { message = $"Welcome {user}!" });
});

await app.Run("http://localhost:5000/");

record CreateUserRequest(string Name, string Email);
```

## 许可证

MIT

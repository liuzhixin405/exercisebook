using MiniGin;

// 创建引擎（类似 gin.Default()）
var app = Gin.Default();

// 启用 Swagger
app.UseSwagger("Mini Gin API", "v1");

// 全局中间件
app.Use(
    Middleware.CORS(),
    Middleware.RequestId()
);

// 根路由
app.GET("/", async ctx => await ctx.String(200, "Mini Gin is ready!"));
app.GET("/ping", async ctx => await ctx.JSON(new { message = "pong" }));

// API 分组
var api = app.Group("/api");
api.Use(ctx =>
{
    ctx.Header("X-Api-Version", "1.0");
    return Task.CompletedTask;
});

// RESTful 风格路由
api.GET("/users", async ctx =>
{
    var page = ctx.Query<int>("page") ?? 1;
    var size = ctx.Query<int>("size") ?? 10;
    await ctx.JSON(new
    {
        users = new[] { new { id = 1, name = "Alice" }, new { id = 2, name = "Bob" } },
        page,
        size
    });
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
        await ctx.BadRequest(new { error = "Invalid request body" });
        return;
    }
    await ctx.Created(new { id = 1, name = user.Name, email = user.Email });
});

api.PUT("/users/:id", async ctx =>
{
    var id = ctx.Param("id");
    var user = await ctx.BindAsync<UpdateUserRequest>();
    await ctx.OK(new { id, updated = true, name = user?.Name });
});

api.DELETE("/users/:id", async ctx =>
{
    var id = ctx.Param("id");
    await ctx.OK(new { id, deleted = true });
});

// 嵌套分组
var admin = api.Group("/admin");
admin.Use(Middleware.BasicAuth((user, pass) => user == "admin" && pass == "123456"));

admin.GET("/dashboard", async ctx =>
{
    var user = ctx.Get<string>("user");
    await ctx.JSON(new { message = $"Welcome {user}!", role = "admin" });
});

// 启动服务器
await app.Run("http://localhost:5000/");

// 请求模型
record CreateUserRequest(string Name, string Email);
record UpdateUserRequest(string? Name, string? Email);
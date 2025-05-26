using OllamaContext7Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 注册HTTP客户端
builder.Services.AddHttpClient<IAIService, AIService>();

// 注册服务
builder.Services.AddScoped<IAIService, AIService>();

// 配置CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// 配置日志
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Information);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 启用CORS
app.UseCors();

app.UseAuthorization();

app.MapControllers();

// 添加健康检查端点
app.MapGet("/health", async (IAIService aiService) =>
{
    var isHealthy = await aiService.CheckHealthAsync();
    return Results.Ok(new
    {
        status = isHealthy ? "healthy" : "unhealthy",
        timestamp = DateTime.UtcNow,
        service = "OllamaContext7Api"
    });
});

app.Run();
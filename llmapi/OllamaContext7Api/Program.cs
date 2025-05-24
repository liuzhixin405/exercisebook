using OllamaContext7Api.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 配置日志
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Ollama Context7 API",
        Version = "v1",
        Description = "结合Context7文档检索的AI问答API"
    });
});

// 配置CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add Services
builder.Services.AddHttpClient<Context7Client>(client =>
{
    client.BaseAddress = new Uri("http://localhost:11434");
    client.Timeout = TimeSpan.FromMinutes(2); // 增加超时时间
});

builder.Services.AddScoped<IAIService, AIService>();
builder.Services.AddSingleton<McpServerService>(); // 单例确保MCP服务器只启动一次

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ollama Context7 API v1");
        c.RoutePrefix = string.Empty; // 让Swagger UI在根路径可用
    });
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// 启动时记录信息
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Ollama Context7 API 启动完成");
logger.LogInformation("Swagger UI 可在 http://localhost:5031 访问");

app.Run();
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AiAgent.Services;
using System;

// 注册编码提供器，支持GB2312等
System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 配置AIServiceOptions
builder.Services.Configure<AIServiceOptions>(
    builder.Configuration.GetSection(AIServiceOptions.SectionName));

// 配置FileServiceOptions
builder.Services.Configure<FileServiceOptions>(
    builder.Configuration.GetSection("FileServiceOptions"));

// 注册HTTP客户端
builder.Services.AddHttpClient<IAIService, AIService>();

// 注册服务
builder.Services.AddScoped<IAIService, AIService>();
builder.Services.AddScoped<IFileService, FileService>();

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

// 添加静态文件支持 - 新增这行
app.UseStaticFiles();

// 添加默认文件支持（可选，让 / 路径自动访问 index.html）
app.UseDefaultFiles();

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
        service = "AiAgent"
    });
});

// 添加根路径重定向到首页（可选）
app.MapGet("/", () => Results.Redirect("/index.html"));

app.Run();

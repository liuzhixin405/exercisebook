using OllamaContext7Api.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ������־
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
        Description = "���Context7�ĵ�������AI�ʴ�API"
    });
});

// ����CORS
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
    client.Timeout = TimeSpan.FromMinutes(2); // ���ӳ�ʱʱ��
});

builder.Services.AddScoped<IAIService, AIService>();
builder.Services.AddSingleton<McpServerService>(); // ����ȷ��MCP������ֻ����һ��

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ollama Context7 API v1");
        c.RoutePrefix = string.Empty; // ��Swagger UI�ڸ�·������
    });
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// ����ʱ��¼��Ϣ
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Ollama Context7 API �������");
logger.LogInformation("Swagger UI ���� http://localhost:5031 ����");

app.Run();
using OllamaContext7Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ע��HTTP�ͻ���
builder.Services.AddHttpClient<IAIService, AIService>();

// ע�����
builder.Services.AddScoped<IAIService, AIService>();

// ����CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ������־
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

// ����CORS
app.UseCors();

app.UseAuthorization();

app.MapControllers();

// ��ӽ������˵�
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
using OllamaContext7Api.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ollama Context7 API", Version = "v1" });
});

// Add Services
builder.Services.AddHttpClient<Context7Client>(client => 
{
    client.BaseAddress = new Uri("http://localhost:11434");
});
builder.Services.AddScoped<IAIService, AIService>();
builder.Services.AddSingleton<McpServerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

using WebSocketServer.Middleware;
using WebSocketManager = WebSocketServer.Middleware.WebSocketManager;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};


builder.Services.AddTransient<WebSocketManager>();
builder.Services.AddTransient<WSHandler>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseWebSockets(webSocketOptions);

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseMiddleware<RealTimeWSMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();

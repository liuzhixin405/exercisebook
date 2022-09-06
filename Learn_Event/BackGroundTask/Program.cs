using BackGroundTask;

var builder = WebApplication.CreateBuilder();
builder.Services.AddTransient<TickerService>();
builder.Services.AddTransient<TransientService>(); //新增生成guid类
builder.Services.AddHostedService<TickerBackGroundService>();
builder.Build().Run();
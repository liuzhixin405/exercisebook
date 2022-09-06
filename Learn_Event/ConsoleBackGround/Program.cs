using ConsoleBackGround;

var builder = WebApplication.CreateBuilder();

builder.Services.AddTransient<TransientService>();  //Guid相同
//builder.Services.AddSingleton<TransientService>(); //构造函数使用Guid相同,使用scope对象注入不了，必须用ATransient
//builder.Services.AddScoped<TransientService>(); //构造函数使用Guid相同, 使用scope对象注入不了,必须用ATransient
builder.Services.AddTransient<TickerService>();

GlobalService.ServiceProvider = builder.Services.BuildServiceProvider();  //一定要在注入之后赋值，要不然只会拿到空对象。
builder.Services.AddHostedService<TickerBackGroundService>();  

builder.Build().Run();
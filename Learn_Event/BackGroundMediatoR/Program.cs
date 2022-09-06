using BackGroundMediatR;
using MediatR;

Console.Title = "BackGroundMediatR";
var builder = WebApplication.CreateBuilder();
//builder.Services.AddSingleton<TransientService>();  //打印相同的guid
builder.Services.AddTransient<TransientService>();  //打印不同的guid
builder.Services.AddMediatR(typeof(Program));

builder.Services.AddHostedService<TickerBackGroundService>();

builder.Build().Run();
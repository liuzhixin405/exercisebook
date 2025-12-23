using FacadeDesgin.Facades;
using FacadeDesgin.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FacadeDesgin API", Version = "v1" });
});

// register services and facade
builder.Services.AddSingleton<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderFacade, OrderFacade>();
builder.Services.AddSingleton<IInventoryService, InventoryService>();
builder.Services.AddSingleton<IPaymentService, PaymentService>();
builder.Services.AddSingleton<IEventPublisher, EventPublisher>();
// register concrete notification senders
builder.Services.AddSingleton<EmailSender>();
builder.Services.AddSingleton<SmsSender>();
builder.Services.AddSingleton<PushSender>();

// provide enumerable of all senders for fallback
builder.Services.AddSingleton<IEnumerable<IMessageSender>>(sp => new IMessageSender[] {
    sp.GetRequiredService<EmailSender>(),
    sp.GetRequiredService<SmsSender>(),
    sp.GetRequiredService<PushSender>()
});

// configure primary sender from configuration (Notification:PrimarySender)
var primary = builder.Configuration["Notification:PrimarySender"];
if (string.Equals(primary, "Sms", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddSingleton<IMessageSender>(sp => sp.GetRequiredService<SmsSender>());
    builder.Services.AddScoped<Func<IMessageSender>>(sp => () => sp.GetRequiredService<SmsSender>());
}
else if (string.Equals(primary, "Push", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddSingleton<IMessageSender>(sp => sp.GetRequiredService<PushSender>());
    builder.Services.AddScoped<Func<IMessageSender>>(sp => () => sp.GetRequiredService<PushSender>());
}
else
{
    builder.Services.AddSingleton<IMessageSender>(sp => sp.GetRequiredService<EmailSender>());
    builder.Services.AddScoped<Func<IMessageSender>>(sp => () => sp.GetRequiredService<EmailSender>());
}

builder.Services.AddScoped<INotificationService, NotificationService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FacadeDesgin API v1"));
}

app.MapControllers();

app.Run();

public partial class Program { }


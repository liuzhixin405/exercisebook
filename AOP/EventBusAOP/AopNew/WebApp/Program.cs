using Common.Bus.Core;
using Common.Bus.Extensions;
using WebApp.Controllers;
using WebApp.Behaviors;
using WebApp.Handlers;
using WebApp.Commands;
namespace WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            // 一次性注册所有CommandBus实现
            builder.Services.AddAllCommandBusImplementations();
            
            // 注册CommandHandlers和CommandProcessors
            builder.Services.AddCommandHandlers();
            
            // 添加实时监控支持
            builder.Services.AddMetricsCollector(TimeSpan.FromSeconds(1));
            
            // 注册管道行为
            builder.Services.AddScoped(typeof(ICommandPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            builder.Services.AddScoped(typeof(ICommandPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            builder.Services.AddScoped(typeof(ICommandPipelineBehavior<,>), typeof(TransactionBehavior<,>));

            // 注册命令处理器
            builder.Services.AddScoped<ICommandHandler<ProcessOrderCommand, string>, ProcessOrderHandler>();
            builder.Services.AddScoped<ICommandHandler<CreateUserCommand, int>, CreateUserHandler>();
            builder.Services.AddScoped<ICommandHandler<SendEmailCommand, bool>, SendEmailHandler>();
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
        }
    }
}

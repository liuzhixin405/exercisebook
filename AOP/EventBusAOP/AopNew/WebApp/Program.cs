using Common.Bus;
using WebApp.Controllers;
using WebApp.Filters;
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
            // 使用强类型数据流CommandBus以获得更好的性能和类型安全
            builder.Services.AddTypedDataflowCommandBus(maxConcurrency: Environment.ProcessorCount * 2);
            
            // 添加实时监控支持
            builder.Services.AddMetricsCollector(TimeSpan.FromSeconds(1));
            
            // 注册管道行为
            builder.Services.AddScoped(typeof(ICommandPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            builder.Services.AddScoped(typeof(ICommandPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            builder.Services.AddScoped(typeof(ICommandPipelineBehavior<,>), typeof(TransactionBehavior<,>));

            // 注册命令处理器
            builder.Services.AddScoped<ICommandHandler<CreateOrderCommand, string>, CreateOrderHandler>();
            builder.Services.AddScoped<ICommandHandler<ProcessOrderCommand, string>, ProcessOrderHandler>();
            
            // 注册强类型命令处理器
            builder.Services.AddScoped<CommandProcessor<CreateOrderCommand, string>>();
            builder.Services.AddScoped<CommandProcessor<ProcessOrderCommand, string>>();
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

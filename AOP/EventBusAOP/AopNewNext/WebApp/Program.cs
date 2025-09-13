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
            // 使用基于时间戳的优化CommandBus（推荐）
            builder.Services.AddTimeBasedCommandBus(
                maxConcurrency: Environment.ProcessorCount * 2,
                enableBatchProcessing: true,
                batchWindowSize: TimeSpan.FromMilliseconds(50)
            );
            
            // 注册管道行为
            builder.Services.AddScoped(typeof(ICommandPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            builder.Services.AddScoped(typeof(ICommandPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            builder.Services.AddScoped(typeof(ICommandPipelineBehavior<,>), typeof(TransactionBehavior<,>));

            // 注册时间基础命令处理器
            builder.Services.AddScoped<ICommandHandler<TimeBasedOrderCommand, TimeBasedOrderResult>, TimeBasedOrderHandler>();
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

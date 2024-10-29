using MassTransit;
using Repository.Service.Consumers;
using Repository.Service.Extensions;
using Repository.Service.Saga;
namespace Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

            builder.Services.AddMysqlServices(Repository.Service.Enums.ServiceType.Order, builder.Configuration);
            builder.Services.AddMysqlServices(Repository.Service.Enums.ServiceType.Product, builder.Configuration);
            builder.Services.AddInternalOrderServices();
            builder.Services.AddInternalProductServices();
            // Add services to the container.
            builder.Services.AddMassTransit(x =>
            {
                x.AddConsumer<CreateOrderConsumer>();
                x.AddConsumer<CreateProductConsumer>();
                x.AddSagaStateMachine<TransactionSaga, TransactionState>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("rabbitmq://localhost:5672", h =>
                    {
                        h.Username("admin");
                        h.Password("admin");
                    });
                });
            });

            builder.Services.AddMassTransitHostedService();
            builder.Services.AddControllers();
            builder.Services.AddInternalSwagger("Api", "v1");
            builder.Services.AddTransient<ApiServicecs>();
            builder.Services.AddHttpClient();
            var app = builder.Build();
            app.UseInternalSwagger("Api", "v1");
            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

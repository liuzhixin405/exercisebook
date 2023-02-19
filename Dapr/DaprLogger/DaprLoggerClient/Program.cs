
using Microsoft.Extensions.Logging;

namespace DaprLoggerClient
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
            builder.Services.AddLogging(builder =>
            {
                builder.AddProvider(new DaprLoggerProvider(new Config
                {
                    LoggerTopic = "logging",
                    PubSubComponent = "pubsub"
                }));
                
            });
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

    public class DaprLoggerProvider : ILoggerProvider
    {
        private readonly Config _config;
        public DaprLoggerProvider(Config config)
        {
            _config = config;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new DLogger(_config);
        }

        public void Dispose()
        {
            
        }
    }
}
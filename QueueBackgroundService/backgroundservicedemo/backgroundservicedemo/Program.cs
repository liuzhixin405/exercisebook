using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace backgroundservicedemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 从配置文件加载 Serilog
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build())
                .CreateLogger();

            try
            {
                Log.Information("Starting application...");

                var builder = WebApplication.CreateBuilder(args);

                // 替换默认日志提供程序为 Serilog
                builder.Host.UseSerilog();

                builder.Services.AddHostedService<ConsoleService>();

                var app = builder.Build();

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application startup failed.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }

    public class ConsoleService : BackgroundService
    {
        private readonly ILogger<ConsoleService> _logger;

        public ConsoleService(ILogger<ConsoleService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Hello World! {DateTime.Now}");
                await Task.Delay(3000, stoppingToken);
            }
        }
    }
}

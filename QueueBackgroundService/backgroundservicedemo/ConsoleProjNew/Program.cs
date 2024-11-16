using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ConsoleProj
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 指定监听的 URL
            builder.WebHost.UseUrls("http://localhost:6000", "https://localhost:6001");

            // 注册服务
            builder.Services.AddSingleton<MonitorLoop>();
            builder.Services.AddHostedService<QueuedHostedService>();
            builder.Services.AddSingleton<IBackgroundTaskQueue>(ctx =>
            {
                if (!int.TryParse(builder.Configuration["QueueCapacity"], out int capacity))
                    capacity = 10;
                return new BackgroundTaskQueue(capacity);
            });

            var app = builder.Build();

            // 启动任务监控
            var monitorLoop = app.Services.GetRequiredService<MonitorLoop>();
            monitorLoop.StartMonitorLoop();

            // 配置简单的路由
            app.MapGet("/", () => "Hello, world!");

            app.Run();
        }
    }

    /// <summary>
    /// 任务生产器
    /// </summary>
    public class MonitorLoop
    {
        private readonly ILogger _logger;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly CancellationToken _cancellationToken;

        public MonitorLoop(IBackgroundTaskQueue taskQueue, ILogger<MonitorLoop> logger, IHostApplicationLifetime applicationLifetime)
        {
            _logger = logger;
            _taskQueue = taskQueue;
            _cancellationToken = applicationLifetime.ApplicationStopping;
        }

        public void StartMonitorLoop()
        {
            _logger.LogInformation("Starting monitor loop.");
            Task.Run(async () => await MonitorAsync());
        }

        private async ValueTask MonitorAsync()
        {
            var random = new Random();

            for (int i = 0; i < 10; i++) // 生成10个任务
            {
                int num1 = random.Next(0, 10001);
                int num2 = random.Next(0, 10001);

                // 将任务绑定数据并入队
                await _taskQueue.QueueBackgroundWorkItemAsync(async token =>
                {
                    var result = num1 + num2;
                    Console.WriteLine($"计算任务：{num1} + {num2} = {result}, 时间：{DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                    await Task.Delay(500, token); // 模拟耗时操作
                });
            }
        }
    }

    /// <summary>
    /// 执行任务的后台服务
    /// </summary>
    public class QueuedHostedService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IBackgroundTaskQueue _taskQueue;

        public QueuedHostedService(ILogger<QueuedHostedService> logger, IBackgroundTaskQueue taskQueue)
        {
            _logger = logger;
            _taskQueue = taskQueue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Queued Hosted Service is running.");

            while (!stoppingToken.IsCancellationRequested)
            {
                // 从队列中获取任务
                var workItem = await _taskQueue.DequeueAsync(stoppingToken);

                try
                {
                    await workItem(stoppingToken); // 执行任务
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred executing task.");
                }
            }

            _logger.LogInformation("Queued Hosted Service is stopping.");
        }
    }

    /// <summary>
    /// 任务队列接口
    /// </summary>
    public interface IBackgroundTaskQueue
    {
        ValueTask QueueBackgroundWorkItemAsync(Func<CancellationToken, ValueTask> workItem);
        ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken);
    }

    /// <summary>
    /// 任务队列实现
    /// </summary>
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly Channel<Func<CancellationToken, ValueTask>> _queue;

        public BackgroundTaskQueue(int capacity)
        {
            var options = new BoundedChannelOptions(capacity)
            {
                FullMode = BoundedChannelFullMode.Wait
            };
            _queue = Channel.CreateBounded<Func<CancellationToken, ValueTask>>(options);
        }

        public async ValueTask QueueBackgroundWorkItemAsync(Func<CancellationToken, ValueTask> workItem)
        {
            if (workItem == null)
                throw new ArgumentNullException(nameof(workItem));

            await _queue.Writer.WriteAsync(workItem);
        }

        public ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken)
        {
            return _queue.Reader.ReadAsync(cancellationToken);
        }
    }
}

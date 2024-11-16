using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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

            // 指定监听的 URL，可以是多个地址
            builder.WebHost.UseUrls("http://localhost:6000", "https://localhost:6001");
            builder.Services.AddSingleton<MonitorLoop>();
            builder.Services.AddHostedService<QueuedHostedService>();
            //builder.Services.AddHostedService<TimedHostedService>();
            builder.Services.AddSingleton<IBackgroundTaskQueue>(ctx =>
            {
                if (!int.TryParse(builder.Configuration["QueueCapacity"], out int capacity))
                    capacity = 10;
                return new BackgroundTaskQueue(capacity);
            });

            var app = builder.Build();
   
            var moniterLoop = app.Services.GetRequiredService<MonitorLoop>();
            moniterLoop.StartMonitorLoop();

            // 配置路由或其他中间件（可选）
            app.MapGet("/", () => "Hello, world!");

            app.Run();
        }
    }

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
            int count = 0;
            while (!_cancellationToken.IsCancellationRequested && count <10 )
            {
                // Wait for a random amount of time between 1 and 5 seconds
                //await Task.Delay(TimeSpan.FromSeconds(new Random().Next(1, 5)));
               
                    // Enqueue a background work item
                    await _taskQueue.QueueBackgroundWorkItemAsync(BuildWorkItem);
                    count++;
            }
        }

        private async ValueTask BuildWorkItem(Data data, CancellationToken token)
        {
            //while (!token.IsCancellationRequested)
            {
                var result = data.Num1 + data.Num2;
                Console.WriteLine($"超级计算进行中:{data.Num1} + {data.Num2} = {result} , 时：{(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss")}秒");
            }
        }
    }
}
/// <summary>
/// 排队后台任务
/// </summary>
public class QueuedHostedService : BackgroundService
{
    private readonly ILogger _logger;
    public QueuedHostedService(ILogger<QueuedHostedService> logger, IBackgroundTaskQueue taskQueue)
    {
        _logger = logger;
        TaskQueue = taskQueue;
    }
    public IBackgroundTaskQueue TaskQueue { get; }
    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
        $"Queued Hosted Service is running.");

        await BackgroundProcessing(stoppingToken);
    }

    private async Task BackgroundProcessing(CancellationToken stoppingToken)
    {
        List<Data> dataList = new List<Data>();
        Random random = new Random();
        for (int i = 0; i < 1000; i++)
        {
            dataList.Add(new Data
            {
                Num1 = random.Next(0, 10001), // 生成0到10000的随机整数
                Num2 = random.Next(0, 10001)
            });
        }
        while (!stoppingToken.IsCancellationRequested)
        {
            var workItem = await TaskQueue.DequeueAsync(stoppingToken); //获取任务 参数应该放到monitor中去，这里只是执行任务不需要具体数据。 要不然monitor就只是一个Task模板
            try
            {
                dataList.AsParallel().ForAll(async d =>await workItem(d, stoppingToken));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred executing {WorkItem}.", workItem);
            }
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Queued Hosted Service is stopping.");

        await base.StopAsync(stoppingToken);
    }
}

public struct Data
{
    public int Num1 { get; set; }
    public int Num2 { get; set; }
}
/// <summary>
/// 类似任务列表，每次能执行多少个任务
/// </summary>
public interface IBackgroundTaskQueue
{
    ValueTask QueueBackgroundWorkItemAsync(Func<Data, CancellationToken, ValueTask> workItem);
    ValueTask<Func<Data, CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken);
}

public class BackgroundTaskQueue : IBackgroundTaskQueue
{

    private readonly Channel<Func<Data, CancellationToken, ValueTask>> _queue;
    public BackgroundTaskQueue(int capacity)
    {
        var options = new BoundedChannelOptions(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait,
        };
        _queue = Channel.CreateBounded<Func<Data, CancellationToken, ValueTask>>(options);
    }
    public async ValueTask QueueBackgroundWorkItemAsync(Func<Data, CancellationToken, ValueTask> workItem)
    {
        if (workItem == null)
        {
            throw new ArgumentNullException(nameof(workItem));
        }

        await _queue.Writer.WriteAsync(workItem);
    }

    public ValueTask<Func<Data, CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken)
    {
        return _queue.Reader.ReadAsync(cancellationToken);
    }
}



public class TimedHostedService : BackgroundService
{
    private readonly ILogger<TimedHostedService> _logger;
    private int _executionCount;

    public TimedHostedService(ILogger<TimedHostedService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 异步定时后台任务
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service running.");

        // When the timer should have no due-time, then do the work once now.
        DoWork();

        using PeriodicTimer timer = new(TimeSpan.FromSeconds(1));

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                DoWork();
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");
        }
    }

    // Could also be a async method, that can be awaited in ExecuteAsync above
    private void DoWork()
    {
        int count = Interlocked.Increment(ref _executionCount);

        _logger.LogInformation("Timed Hosted Service is working. Count: {Count}", count);
    }
}
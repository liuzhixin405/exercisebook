using System.Diagnostics.Metrics;
using OpenTelemetry.Metrics;

const string MeterName = "DemoMetrics";

var builder = WebApplication.CreateBuilder(args);

var meter = new Meter(MeterName, "1.0.0");
var orders = meter.CreateCounter<long>("demo.orders.count", "{order}", "处理的订单数");
var duration = meter.CreateHistogram<double>("demo.order.duration", "ms", "订单处理耗时");
var active = meter.CreateUpDownCounter<int>("demo.active.requests", "{request}");
int queueLength = 0;
meter.CreateObservableGauge("demo.queue.length", () => Volatile.Read(ref queueLength), "{item}", "队列长度");

builder.Services.AddOpenTelemetry()
    .WithMetrics(m => m
        .AddMeter(MeterName)
        .AddMeter("Microsoft.AspNetCore.Hosting", "System.Runtime")   // 框架内置指标
        .AddPrometheusExporter());

var app = builder.Build();

// Prometheus 抓取端点：/metrics
app.MapPrometheusScrapingEndpoint();

app.MapGet("/", () => "访问 /order 产生指标，访问 /metrics 查看 Prometheus 输出");
app.MapGet("/order", async () =>
{
    var region = new KeyValuePair<string, object?>("region", new[] { "cn", "us", "eu" }[Random.Shared.Next(3)]);
    active.Add(1);
    Interlocked.Increment(ref queueLength);
    try
    {
        var ms = Random.Shared.Next(10, 200);
        await Task.Delay(ms);
        orders.Add(1, region);
        duration.Record(ms, region);
        return Results.Ok(new { region = region.Value, ms });
    }
    finally
    {
        Interlocked.Decrement(ref queueLength);
        active.Add(-1);
    }
});

app.Run();

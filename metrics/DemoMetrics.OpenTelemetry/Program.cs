using System.Diagnostics.Metrics;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

const string MeterName = "DemoMetrics";

// OpenTelemetry MeterProvider：订阅 Meter，每 2 秒导出到控制台
using var meterProvider = Sdk.CreateMeterProviderBuilder()
    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("DemoMetrics.OpenTelemetry"))
    .AddMeter(MeterName)
    .AddConsoleExporter((exporterOptions, readerOptions) =>
    {
        readerOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 2000;
    })
    .Build();

using var meter = new Meter(MeterName, "1.0.0");
var orders = meter.CreateCounter<long>("demo.orders.count", "{order}", "处理的订单数");
var duration = meter.CreateHistogram<double>("demo.order.duration", "ms", "订单处理耗时");
var active = meter.CreateUpDownCounter<int>("demo.active.requests", "{request}");
var queue = new Queue<int>();
meter.CreateObservableGauge("demo.queue.length", () => queue.Count, "{item}", "队列长度");

var rnd = new Random();
string[] regions = ["cn", "us", "eu"];
for (int i = 0; i < 20; i++)
{
    active.Add(1);
    queue.Enqueue(i);
    var region = new KeyValuePair<string, object?>("region", regions[rnd.Next(regions.Length)]);
    await Task.Delay(300);
    orders.Add(1, region);
    duration.Record(rnd.NextDouble() * 100, region);
    if (i % 2 == 1) queue.Dequeue();
    active.Add(-1);
}
// Dispose 时会 flush 最后一批数据

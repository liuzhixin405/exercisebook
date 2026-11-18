using System.Diagnostics.Metrics;

// 定义 Meter 与各类 Instrument
using var meter = new Meter("DemoMetrics", "1.0.0");
var ordersCounter = meter.CreateCounter<long>("demo.orders.count", unit: "{order}", description: "处理的订单数");
var durationHistogram = meter.CreateHistogram<double>("demo.order.duration", unit: "ms", description: "订单处理耗时");
var queue = new Queue<int>();
meter.CreateObservableGauge("demo.queue.length", () => queue.Count, unit: "{item}", description: "队列长度");
var upDown = meter.CreateUpDownCounter<int>("demo.active.requests", unit: "{request}");

// 用 MeterListener 在进程内收集并打印指标
using var listener = new MeterListener();
listener.InstrumentPublished = (inst, l) =>
{
    if (inst.Meter.Name == "DemoMetrics") l.EnableMeasurementEvents(inst);
};
listener.SetMeasurementEventCallback<long>(Print);
listener.SetMeasurementEventCallback<double>(Print);
listener.SetMeasurementEventCallback<int>(Print);
listener.Start();

static void Print<T>(Instrument inst, T value, ReadOnlySpan<KeyValuePair<string, object?>> tags, object? state) where T : struct
{
    var tagText = string.Join(",", tags.ToArray().Select(t => $"{t.Key}={t.Value}"));
    Console.WriteLine($"[{inst.Name}] {value} {inst.Unit} {{{tagText}}}");
}

var rnd = new Random(42);
string[] regions = ["cn", "us", "eu"];
for (int i = 0; i < 10; i++)
{
    upDown.Add(1);
    queue.Enqueue(i);
    var region = regions[rnd.Next(regions.Length)];
    var ms = rnd.NextDouble() * 100;
    await Task.Delay(50);
    ordersCounter.Add(1, new KeyValuePair<string, object?>("region", region));
    durationHistogram.Record(ms, new KeyValuePair<string, object?>("region", region));
    if (i % 2 == 1) queue.Dequeue();
    upDown.Add(-1);
}

Console.WriteLine("--- 采集 Observable 仪表 ---");
listener.RecordObservableInstruments();

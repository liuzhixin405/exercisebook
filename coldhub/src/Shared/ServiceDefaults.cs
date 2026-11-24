using System.Collections.Concurrent;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ColdHub.Shared;

/// <summary>
/// 每个进程的通用设施：
/// /health/live 只表示进程活着，/health/ready 汇总带 "ready" 标签的依赖检查，/metrics 以文本输出本进程的指标。
/// 生产里通常用 OpenTelemetry 导出；这里用 MeterListener 手写一个最小版，好看清指标是怎么流出来的。
/// </summary>
public static class ServiceDefaults
{
    public const string ReadyTag = "ready";

    public static TBuilder AddServiceDefaults<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Services.AddHealthChecks();
        builder.Services.AddSingleton<MetricsText>();

        // 指标监听必须在各个 Meter 开始计数之前就建好，否则之前的计数丢了；所以不等第一次抓取，随宿主启动就创建
        builder.Services.AddHostedService<MetricsStarter>();
        return builder;
    }

    public static IEndpointRouteBuilder MapDefaultEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapHealthChecks("/health/live", new HealthCheckOptions { Predicate = _ => false });
        app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = check => check.Tags.Contains(ReadyTag) });
        app.MapGet("/metrics", (MetricsText metrics) => Results.Text(metrics.Render(), "text/plain; version=0.0.4"));
        return app;
    }
}

/// <summary>让 DI 在宿主启动时就创建 <see cref="MetricsText"/>，并每 10 秒把指标快照写入日志。</summary>
internal sealed class MetricsStarter(MetricsText metrics, ILogger<MetricsStarter> logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(10);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Interval);
        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                string snapshot = metrics.Render();
                if (snapshot.Length > 0)
                {
                    logger.LogInformation("Metrics:{NewLine}{Snapshot}", Environment.NewLine, snapshot);
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
    }
}

/// <summary>
/// 监听本进程所有以 ColdHub 开头的 Meter（约定：Meter 名 ColdHub.&lt;Area&gt;，指标名 coldhub.&lt;area&gt;.&lt;thing&gt;），
/// 计数器累加，可观测的 gauge 在每次抓取时现读。
/// </summary>
public sealed class MetricsText : IDisposable
{
    private readonly MeterListener _listener = new();
    private readonly ConcurrentDictionary<string, double> _values = new();

    public MetricsText()
    {
        _listener.InstrumentPublished = (instrument, listener) =>
        {
            if (instrument.Meter.Name.StartsWith("ColdHub", StringComparison.Ordinal))
            {
                listener.EnableMeasurementEvents(instrument);
            }
        };
        _listener.SetMeasurementEventCallback<long>(Record);
        _listener.SetMeasurementEventCallback<int>(Record);
        _listener.SetMeasurementEventCallback<double>(Record);
        _listener.Start();
    }

    public string Render()
    {
        _listener.RecordObservableInstruments();
        var text = new StringBuilder();

        foreach ((string series, double value) in _values.OrderBy(pair => pair.Key, StringComparer.Ordinal))
        {
            text.Append(series).Append(' ').AppendLine(value.ToString(CultureInfo.InvariantCulture));
        }

        return text.ToString();
    }

    public void Dispose() => _listener.Dispose();

    private void Record<T>(Instrument instrument, T measurement, ReadOnlySpan<KeyValuePair<string, object?>> tags,
        object? state) where T : struct
    {
        double value = Convert.ToDouble(measurement, CultureInfo.InvariantCulture);
        string name = instrument.Name.Replace('.', '_');
        string series = tags.IsEmpty
            ? name
            : $"{name}{{{string.Join(',', tags.ToArray().Select(tag => $"{tag.Key.Replace('.', '_')}=\"{tag.Value}\""))}}}";

        // 计数器是增量，累加；可观测 gauge 每次给的是当前值，覆盖
        if (instrument.IsObservable)
        {
            _values[series] = value;
        }
        else
        {
            _values.AddOrUpdate(series, value, (_, old) => old + value);
        }
    }
}

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ColdHub.Shared;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace ColdHub.Source;

/// <summary>Admin 下发的配置，字段与 Admin 的 SourceConfig 一致。</summary>
public sealed record SourceConfig(List<string> Devices, int IntervalMs, int MaxPerSecond);

/// <summary>最近一次拿到的配置。volatile：ConfigWatcher 线程写，SourceWorker 线程读。</summary>
public sealed class ConfigHolder
{
    private volatile SourceConfig? _current;

    public SourceConfig? Current => _current;

    /// <summary>Admin 说这个源已退役（410）：停止发布。</summary>
    public volatile bool Retired;

    public void Set(SourceConfig config) => _current = config;
}

/// <summary>
/// 轮询 Admin 取配置并热更新，带 ETag：没变化 Admin 回 304。
/// Admin 连不上时保留上一份配置继续发布，并按"故障开始一条 Warning、结束一条 Information"的规则记日志。
/// </summary>
public sealed class ConfigWatcher(HttpClient admin, IOptions<SourceOptions> options, ConfigHolder holder,
    Publishing.ReadingPublisher publisher, ILogger<ConfigWatcher> logger) : BackgroundService
{
    private static readonly TimeSpan Every = TimeSpan.FromSeconds(2);

    private string? _etag;
    private bool _failing;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Every);

        do
        {
            await PollAsync(stoppingToken);
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    internal async Task PollAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, $"sources/{options.Value.Name}/config");

            if (_etag is not null)
            {
                request.Headers.IfNoneMatch.Add(EntityTagHeaderValue.Parse(_etag));
            }

            using HttpResponseMessage response = await admin.SendAsync(request, cancellationToken);

            switch (response.StatusCode)
            {
                case HttpStatusCode.NotModified:
                    break;
                case HttpStatusCode.Gone:
                    holder.Retired = true;
                    logger.LogWarning("Source {Source} is retired in Admin; publishing stops", options.Value.Name);
                    break;
                default:
                    response.EnsureSuccessStatusCode();
                    var config = await response.Content.ReadFromJsonAsync<SourceConfig>(cancellationToken)
                                 ?? throw new InvalidDataException("empty config");
                    _etag = response.Headers.ETag?.ToString();
                    holder.Retired = false;
                    holder.Set(config);
                    publisher.Configure(config);
                    logger.LogInformation("Config {Version}: {Devices} devices every {Interval} ms, at most {Max}/s",
                        _etag, config.Devices.Count, config.IntervalMs, config.MaxPerSecond);
                    break;
            }

            if (_failing)
            {
                _failing = false;
                logger.LogInformation("Admin is reachable again");
            }
        }
        catch (Exception e) when (e is HttpRequestException or InvalidDataException or TaskCanceledException
                                      && !cancellationToken.IsCancellationRequested)
        {
            if (!_failing)
            {
                _failing = true;
                logger.LogWarning(e, "Cannot get the config from Admin; keeping the last one, trying again every {Seconds} s",
                    Every.TotalSeconds);
            }
        }
    }
}

/// <summary>就绪检查：一个依赖一个检查，各自注册（对应 CLAUDE.md "A dependency registers its own readiness check"）。</summary>
public sealed class ConfigReadyCheck(ConfigHolder holder) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) =>
        Task.FromResult(holder.Current is null
            ? HealthCheckResult.Unhealthy("no config from Admin yet")
            : HealthCheckResult.Healthy());
}

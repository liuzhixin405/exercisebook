using System.Net.Http.Json;
using ColdHub.Shared;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ColdHub.Hub;

/// <summary>
/// 有效 API key 的哈希集合，定时从 Admin 取，在内存里校验客户端带来的 key。
/// 这样 Hub 的每次请求都不用去问 Admin；代价是在 Admin 里吊销一把 key，最多要等一个轮询周期才对 Hub 生效。
/// Admin 暂时连不上时沿用上一份（已有的客户端不受影响）；一份都没拿到过则一律拒绝（宁可拒绝也不放行）。
/// </summary>
public sealed class KeyDirectory(IHttpClientFactory clients, ILogger<KeyDirectory> logger) : BackgroundService
{
    public const string AdminClient = "admin";

    private readonly HttpClient _admin = clients.CreateClient(AdminClient);

    private static readonly TimeSpan Every = TimeSpan.FromSeconds(5);

    private volatile HashSet<string>? _hashes;
    private bool _failing;

    public bool Loaded => _hashes is not null;

    public bool IsValid(string? key) => key is { Length: > 0 } && _hashes?.Contains(ApiKeys.Hash(key)) == true;

    public bool IsValidRequest(HttpRequest request) => IsValid(ApiKeys.FromHeader(request.Headers.Authorization));

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Every);

        do
        {
            try
            {
                string[] hashes = await _admin.GetFromJsonAsync<string[]>("keys/hashes", stoppingToken) ?? [];
                _hashes = [.. hashes];

                if (_failing)
                {
                    _failing = false;
                    logger.LogInformation("Admin is reachable again; {Count} keys", hashes.Length);
                }
            }
            catch (Exception e) when (e is HttpRequestException or TaskCanceledException && !stoppingToken.IsCancellationRequested)
            {
                if (!_failing)
                {
                    _failing = true;
                    logger.LogWarning(e, "Cannot get the API keys from Admin; keeping the last set (none yet: everything is refused)");
                }
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }
}

public sealed class KeysReadyCheck(KeyDirectory keys) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) =>
        Task.FromResult(keys.Loaded ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy("no API keys from Admin yet"));
}

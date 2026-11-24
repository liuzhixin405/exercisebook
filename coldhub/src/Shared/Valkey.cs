using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace ColdHub.Shared;

public static class ValkeyExtensions
{
    /// <summary>
    /// 注册到 Valkey 的连接（连接串在 ConnectionStrings:valkey）和它的就绪检查。
    /// 连不上也要能启动（AbortOnConnectFail=false），客户端在后台重连，就绪检查如实反映，而不是让进程崩溃。
    /// 这里的 Valkey 就是 Redis：本地装的 Redis 直接可用。
    /// </summary>
    public static IServiceCollection AddValkey(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("valkey")
                                  ?? throw new InvalidOperationException("ConnectionStrings:valkey is not set");
        ConfigurationOptions options = ConfigurationOptions.Parse(connectionString);
        options.AbortOnConnectFail = false;
        options.ConnectTimeout = 2000;

        services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(options));
        services.AddHealthChecks().AddCheck<ValkeyReadyCheck>("valkey", tags: [ServiceDefaults.ReadyTag]);
        return services;
    }
}

public sealed class ValkeyReadyCheck(IConnectionMultiplexer valkey) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(valkey.IsConnected ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy("Valkey is down"));
}

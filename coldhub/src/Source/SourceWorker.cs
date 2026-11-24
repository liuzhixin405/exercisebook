using ColdHub.Source.Connectors;
using ColdHub.Source.Publishing;

namespace ColdHub.Source;

/// <summary>
/// 驱动循环：按配置的间隔，让连接器读每个设备，交给发布方。配置每轮重新读，所以 Admin 里改了设备清单或间隔，
/// 不用重启进程就生效。
/// </summary>
public sealed class SourceWorker(IConnector connector, ConfigHolder config, ReadingPublisher publisher, TimeProvider time)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                SourceConfig? current = config.Current;

                if (current is null || config.Retired)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(500), time, stoppingToken);
                    continue;
                }

                DateTimeOffset now = time.GetUtcNow();

                foreach (string device in current.Devices)
                {
                    RawReading raw = connector.Read(device, now);
                    publisher.Publish(device, raw.SensorTime, raw.Probes);
                }

                await Task.Delay(TimeSpan.FromMilliseconds(current.IntervalMs), time, stoppingToken);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
        }
    }
}

using System.Collections.Concurrent;
using System.Diagnostics.Metrics;
using ColdHub.Shared;
using ColdHub.Source.Valkey;
using Microsoft.Extensions.Options;

namespace ColdHub.Source.Publishing;

/// <summary>
/// 把连接器读到的数据校验、编码、发到设备的通道上。永不抛异常：连接器每读到一条都交给它，
/// 不能因为某一条发不出去而停下。
///
/// 三种"没发出去"各有不同的对待：
///   empty       没有探头数据：很常见，静默丢弃，只计数。
///   invalid     编码器拒绝（探头乱序、低于绝对零度）：每个设备开始出错时记一条 Warning，恢复时记一条 Information。
///   unreachable Valkey 连不上或跟不上：整个进程记一条 Warning，恢复时记一条 Information。
/// 因为"每次都记"在数据坏掉时会一秒几十条，把真正要看的日志淹没。
/// </summary>
public sealed class ReadingPublisher
{
    public const string MeterName = "ColdHub.Source";
    public const sbyte Exponent = -2;

    private static readonly string[] AlertingReasons = ["invalid", "unreachable"];

    private readonly IReadingPublisher _publisher;
    private readonly TimeProvider _time;
    private readonly ILogger<ReadingPublisher> _logger;
    private readonly string _source;
    private readonly PublishThrottle _throttle;
    private readonly Counter<long> _published;
    private readonly Counter<long> _dropped;

    // 上一条读数被编码器拒绝的设备：只在状态变化时记日志
    private readonly ConcurrentDictionary<string, byte> _refused = new();
    private int _unreachable;

    public ReadingPublisher(IReadingPublisher publisher, TimeProvider time, ILogger<ReadingPublisher> logger,
        IMeterFactory meterFactory, IOptions<SourceOptions> options)
    {
        _publisher = publisher;
        _time = time;
        _logger = logger;
        _source = options.Value.Name;
        _throttle = new PublishThrottle(time);

        Meter meter = meterFactory.Create(MeterName);
        _published = meter.CreateCounter<long>("coldhub.source.readings.published", "{reading}", "Readings put on the pub/sub.");
        _dropped = meter.CreateCounter<long>("coldhub.source.readings.dropped", "{reading}",
            "Readings not published, by reason: empty, invalid, unreachable, throttled.");

        // 告警读的序列从启动起就以 0 存在：计数器的第一条序列要等第一次增量才出现，
        // 而 increase() 对"从大于 0 开始"的序列会漏掉第一次增量
        foreach (string reason in AlertingReasons)
        {
            _dropped.Add(0, new KeyValuePair<string, object?>("coldhub.drop.reason", reason));
        }
    }

    public void Configure(SourceConfig config) => _throttle.Set(config.MaxPerSecond);

    /// <returns>是否被接受：发布了，或因太快被限流跳过；其他原因没发出去返回 false。</returns>
    public bool Publish(string device, long? sensorTime, ReadOnlySpan<Probe> probes)
    {
        if (probes.IsEmpty)
        {
            Drop("empty");
            return false;
        }

        long publishTime = UnixTime.ToNanoseconds(_time.GetUtcNow());
        byte[] message;

        try
        {
            message = ReadingCodec.Encode(_source, device, Guid.CreateVersion7(_time.GetUtcNow()),
                sensorTime ?? publishTime, publishTime, Exponent, probes);
        }
        catch (ArgumentException e)
        {
            if (_refused.TryAdd(device, 0))
            {
                _logger.LogWarning("Readings of {Device} cannot be published until they are valid again: {Reason}",
                    device, e.Message);
            }

            Drop("invalid");
            return false;
        }

        if (_refused.TryRemove(device, out _))
        {
            _logger.LogInformation("Readings of {Device} are valid again", device);
        }

        if (!_throttle.Allows(device))
        {
            Drop("throttled");
            return true;
        }

        try
        {
            _publisher.Publish(Channels.For(_source, device), message);
        }
        catch (Exception e)
        {
            if (Interlocked.Exchange(ref _unreachable, 1) == 0)
            {
                _logger.LogWarning(e, "Readings cannot be published: the pub/sub is not reachable");
            }

            Drop("unreachable");
            return false;
        }

        if (Interlocked.Exchange(ref _unreachable, 0) == 1)
        {
            _logger.LogInformation("Readings are being published again");
        }

        _published.Add(1);
        return true;
    }

    private void Drop(string reason) => _dropped.Add(1, new KeyValuePair<string, object?>("coldhub.drop.reason", reason));
}

/// <summary>每个设备最多每秒 N 条：距离上一条发布不足 1/N 秒的直接跳过（不延后补发）。0 表示不限。</summary>
public sealed class PublishThrottle(TimeProvider time)
{
    private readonly ConcurrentDictionary<string, long> _last = new();
    private volatile int _maxPerSecond;

    public void Set(int maxPerSecond) => _maxPerSecond = maxPerSecond;

    public bool Allows(string device)
    {
        int max = _maxPerSecond;

        if (max <= 0)
        {
            return true;
        }

        long now = time.GetTimestamp();
        TimeSpan gap = TimeSpan.FromSeconds(1.0 / max);

        // 多线程同时进来时可能多放过一条，对限流来说可以接受
        if (_last.TryGetValue(device, out long last) && time.GetElapsedTime(last, now) < gap)
        {
            return false;
        }

        _last[device] = now;
        return true;
    }
}

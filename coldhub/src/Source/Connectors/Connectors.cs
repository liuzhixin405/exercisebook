using ColdHub.Shared;

namespace ColdHub.Source.Connectors;

/// <summary>连接器读到的一条原始数据。SensorTime 为空表示设备没给时间，发布时用发布时刻。</summary>
public readonly record struct RawReading(long? SensorTime, Probe[] Probes);

/// <summary>
/// 一种数据来源的接入方式。真实场景里是对接设备或网关的驱动（订阅 + 解析）；
/// 这里用模拟：每个设备按需"读"一次。新增一种来源 = 实现这个接口 + 在 <see cref="ConnectorCatalog"/> 登记一行。
/// </summary>
public interface IConnector
{
    RawReading Read(string device, DateTimeOffset now);
}

/// <summary>连接器目录：Source__Connector 的值必须是这里的 key。</summary>
public static class ConnectorCatalog
{
    private static readonly Dictionary<string, Func<IConnector>> Connectors = new(StringComparer.Ordinal)
    {
        ["sine"] = () => new SineConnector(),
        ["random"] = () => new RandomConnector()
    };

    public static IEnumerable<string> Names => Connectors.Keys;

    public static IConnector Create(string name) => Connectors.TryGetValue(name, out Func<IConnector>? create)
        ? create()
        : throw new InvalidOperationException(
            $"unknown connector '{name}'; known: {string.Join(", ", Connectors.Keys)}");
}

/// <summary>规规矩矩的正弦波温度：3 个探头，围绕 4℃ 缓慢起伏。永远是合法数据。</summary>
internal sealed class SineConnector : IConnector
{
    public RawReading Read(string device, DateTimeOffset now)
    {
        double phase = (uint)device.GetHashCode() % 100 / 10.0;
        double t = now.ToUnixTimeMilliseconds() / 5000.0 + phase;

        // 指数 -2：4.25℃ 存成 425
        return new RawReading(null,
        [
            new Probe(1, (long)Math.Round((4 + 1.5 * Math.Sin(t)) * 100)),
            new Probe(2, (long)Math.Round((4.3 + 1.5 * Math.Sin(t + 0.2)) * 100)),
            new Probe(3, (long)Math.Round((4.6 + 1.5 * Math.Sin(t + 0.4)) * 100))
        ]);
    }
}

/// <summary>
/// 会"出毛病"的传感器：每个设备时不时进入一段故障期（持续约 10 次读数），故障有三种：
/// 读不到探头、探头编号乱序、读数低于绝对零度。故障是成段的而不是逐条随机的，
/// 这样才能看到发布方"故障开始记一条、恢复记一条"，而不是每次抖动都记。
/// </summary>
internal sealed class RandomConnector : IConnector
{
    private enum Fault { None, Empty, Unordered, BelowZero }

    private readonly Dictionary<string, (Fault Kind, int Left)> _faults = [];

    public RawReading Read(string device, DateTimeOffset now)
    {
        (Fault kind, int left) = _faults.GetValueOrDefault(device);

        if (left > 0)
        {
            _faults[device] = (kind, left - 1);
        }
        else if (Random.Shared.NextDouble() < 0.04)
        {
            kind = (Fault)Random.Shared.Next(1, 4);
            _faults[device] = (kind, 10);
        }
        else
        {
            kind = Fault.None;
        }

        long Around(double centre) => (long)Math.Round((centre + (Random.Shared.NextDouble() - 0.5) * 2) * 100);

        return kind switch
        {
            Fault.Empty => new RawReading(null, []),
            Fault.Unordered => new RawReading(null, [new Probe(2, Around(-18)), new Probe(1, Around(-18))]),
            Fault.BelowZero => new RawReading(null, [new Probe(1, -30000)]),
            _ => new RawReading(now.ToUnixTimeMilliseconds() * 1_000_000,
                [new Probe(1, Around(-18)), new Probe(2, Around(-17.5))])
        };
    }
}

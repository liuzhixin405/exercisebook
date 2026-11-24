using System.Text.RegularExpressions;

namespace ColdHub.Shared;

/// <summary>
/// 一个探头在某一时刻的温度。真实温度 = Mantissa × 10^Exponent，整数存储避免浮点误差。
/// </summary>
public readonly record struct Probe(byte Id, long Mantissa);

/// <summary>
/// 一条读数：一个冷库（Device）上所有探头在同一时刻的温度。新的读数整体覆盖旧的。
/// </summary>
public sealed record Reading(string Source, string Device, Guid Id, long SensorTime, long PublishTime, sbyte Exponent,
    Probe[] Probes)
{
    public decimal Temperature(Probe probe) => probe.Mantissa * Pow10(Exponent);

    internal static decimal Pow10(int exponent)
    {
        decimal scale = 1m;

        for (int i = 0; i < Math.Abs(exponent); i++)
        {
            scale = exponent < 0 ? scale / 10 : scale * 10;
        }

        return scale;
    }
}

/// <summary>Redis 通道名：<c>Reading:{Source}:{Device}</c>。</summary>
public static partial class Channels
{
    /// <summary>Recorder 用它一次订阅所有通道。</summary>
    public const string All = "Reading:*";

    public static string For(string source, string device) => $"Reading:{source}:{device}";

    /// <summary>名字里不许出现 ':' 和 '*'，否则通道名会有歧义或被当成通配符。</summary>
    public static bool IsValidName(string? name) => name is not null && NameRule().IsMatch(name);

    [GeneratedRegex("^[A-Za-z0-9._-]{1,64}$")]
    private static partial Regex NameRule();
}

public static class UnixTime
{
    /// <summary>Unix 纪元以来的纳秒，线上格式里所有时间字段的单位。</summary>
    public static long ToNanoseconds(DateTimeOffset time) => (time.UtcTicks - DateTime.UnixEpoch.Ticks) * 100;

    public static DateTimeOffset FromNanoseconds(long nanoseconds) =>
        new(DateTime.UnixEpoch.Ticks + nanoseconds / 100, TimeSpan.Zero);
}

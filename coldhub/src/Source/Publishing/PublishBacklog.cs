namespace ColdHub.Source.Publishing;

/// <summary>
/// 积压是否过多。到达 <see cref="High"/> 算"超限"，要降到 <see cref="Low"/> 以下才恢复（滞回）：
/// 只用一条线的话，积压在线附近抖动会让"故障开始/结束"每次检查都翻转。
/// 读积压要遍历所有连接，所以最多每 <see cref="CheckEvery"/> 读一次，不是每条消息都读。
/// </summary>
public sealed class PublishBacklog(Func<long> waiting, TimeProvider time, long high = PublishBacklog.High,
    long low = PublishBacklog.Low)
{
    public const long High = 5_000;
    public const long Low = 500;
    public static readonly TimeSpan CheckEvery = TimeSpan.FromMilliseconds(10);

    private long _checkedAt;
    private bool _checked;
    private long _waiting;
    private volatile bool _over;

    public long Waiting => Volatile.Read(ref _waiting);

    /// <summary>任意线程可调；偶尔两个线程同时读一次计数，无害。</summary>
    public bool IsOver()
    {
        long now = time.GetTimestamp();

        if (!Volatile.Read(ref _checked) || time.GetElapsedTime(Volatile.Read(ref _checkedAt), now) >= CheckEvery)
        {
            Volatile.Write(ref _checkedAt, now);
            Volatile.Write(ref _checked, true);

            long count = waiting();
            Volatile.Write(ref _waiting, count);

            if (count >= high)
            {
                _over = true;
            }
            else if (count <= low)
            {
                _over = false;
            }
        }

        return _over;
    }
}

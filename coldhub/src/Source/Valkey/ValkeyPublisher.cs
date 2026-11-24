using System.Diagnostics.Metrics;
using ColdHub.Source.Publishing;
using StackExchange.Redis;

namespace ColdHub.Source.Valkey;

/// <summary>把消息放到一个通道上的端口。实现在这里是 Valkey；测试里可以换成假的。</summary>
public interface IReadingPublisher
{
    void Publish(string channel, ReadOnlyMemory<byte> message);
}

/// <summary>
/// 发后不管（FireAndForget）：每个读数都等回复，等于每次都多一个网络往返。代价是断线时命令会被静默丢掉，
/// 所以先检查连接，断线就抛异常让调用方能记录；Valkey 卡住但连接还在时，客户端会无限缓冲命令，
/// 所以积压过多也抛异常——丢旧数据没关系，同一设备马上会有新读数。
/// </summary>
public sealed class ValkeyPublisher : IReadingPublisher
{
    private readonly IConnectionMultiplexer _connection;
    private readonly ISubscriber _subscriber;
    private readonly PublishBacklog _backlog;

    public ValkeyPublisher(IConnectionMultiplexer connection, TimeProvider time, IMeterFactory meterFactory)
    {
        _connection = connection;
        _subscriber = connection.GetSubscriber();
        _backlog = new PublishBacklog(() => connection.GetCounters().TotalOutstanding, time);

        Meter meter = meterFactory.Create(ReadingPublisher.MeterName);
        meter.CreateObservableGauge("coldhub.source.publish.backlog", () => _backlog.Waiting, "{command}",
            "Commands waiting for Valkey; readings are dropped while too many are.");
    }

    public void Publish(string channel, ReadOnlyMemory<byte> message)
    {
        if (!_connection.IsConnected)
        {
            throw new InvalidOperationException("not connected to Valkey");
        }

        if (_backlog.IsOver())
        {
            throw new InvalidOperationException($"Valkey is not keeping up: {_backlog.Waiting} commands waiting");
        }

        _subscriber.Publish(RedisChannel.Literal(channel), message, CommandFlags.FireAndForget);
    }
}

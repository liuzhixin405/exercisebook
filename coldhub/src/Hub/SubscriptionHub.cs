using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics.Metrics;
using StackExchange.Redis;

namespace ColdHub.Hub;

/// <summary>
/// 一个通道，不管有多少客户端订阅，Hub 只向 Valkey 订阅一次，收到后扇出给所有订阅它的客户端。
/// 第一个客户端订阅时才去订 Valkey，最后一个走了就退订。
///
/// 热路径（Valkey 回调 → 扇出）不加锁：每个通道的客户端集合是不可变数组，增减时整体替换（写时复制）。
/// 订阅和退订（冷路径）串行化，避免同一个通道"正在退订又来订阅"的竞争。
/// </summary>
public sealed class SubscriptionHub
{
    private readonly ISubscriber _valkey;
    private readonly ConcurrentDictionary<string, ImmutableArray<ClientSession>> _topics = new();
    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly Counter<long> _forwarded;

    public SubscriptionHub(IConnectionMultiplexer valkey, IMeterFactory meterFactory)
    {
        _valkey = valkey.GetSubscriber();

        Meter meter = meterFactory.Create("ColdHub.Hub");
        _forwarded = meter.CreateCounter<long>("coldhub.hub.messages.forwarded", "{message}", "Messages queued for a client.");
        meter.CreateObservableGauge("coldhub.hub.topics", () => _topics.Count, "{channel}", "Channels subscribed on Valkey.");
    }

    public int Topics => _topics.Count;

    public async Task SubscribeAsync(ClientSession session, string channel)
    {
        await _gate.WaitAsync();

        try
        {
            bool first = !_topics.ContainsKey(channel);
            ImmutableArray<ClientSession> current = _topics.GetValueOrDefault(channel, []);

            if (current.Contains(session))
            {
                return;
            }

            _topics[channel] = current.Add(session);

            if (first)
            {
                await _valkey.SubscribeAsync(RedisChannel.Literal(channel), (_, value) => Fanout(channel, value));
            }
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task UnsubscribeAsync(ClientSession session, string channel)
    {
        await _gate.WaitAsync();

        try
        {
            if (!_topics.TryGetValue(channel, out ImmutableArray<ClientSession> current))
            {
                return;
            }

            ImmutableArray<ClientSession> left = current.Remove(session);

            if (left.IsEmpty)
            {
                _topics.TryRemove(channel, out _);
                await _valkey.UnsubscribeAsync(RedisChannel.Literal(channel));
            }
            else
            {
                _topics[channel] = left;
            }
        }
        finally
        {
            _gate.Release();
        }
    }

    /// <summary>客户端断开：退掉它订阅的所有通道。</summary>
    public async Task RemoveAsync(ClientSession session)
    {
        foreach (string channel in _topics.Where(topic => topic.Value.Contains(session)).Select(topic => topic.Key))
        {
            await UnsubscribeAsync(session, channel);
        }
    }

    private void Fanout(string channel, RedisValue value)
    {
        if (!_topics.TryGetValue(channel, out ImmutableArray<ClientSession> sessions))
        {
            return;
        }

        // 所有客户端共用同一份字节，不复制
        byte[] message = (byte[])value!;

        foreach (ClientSession session in sessions)
        {
            session.Enqueue(message);
        }

        _forwarded.Add(sessions.Length);
    }
}

using System.Diagnostics.Metrics;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Channels;

namespace ColdHub.Hub;

/// <summary>
/// 一个 WebSocket 客户端。
///
/// 每个客户端一条有界队列（容量 <see cref="Capacity"/>），满了丢最旧的：慢客户端只会让自己丢数据，
/// 不会让 Valkey 回调线程阻塞，更不会拖慢别的客户端。读数是"最新状态"型数据，丢旧的无妨。
/// 所有发送都经过发送循环：WebSocket 同一时刻只允许一个发送，应答和数据不能各发各的。
/// 客户端 → 服务端是 JSON 文本 {"op":"subscribe","channels":[...]}，服务端 → 客户端的读数是原样转发的二进制帧。
/// </summary>
public sealed partial class ClientSession
{
    public const int Capacity = 10;

    private readonly WebSocket _socket;
    private readonly SubscriptionHub _hub;
    private readonly TimeSpan _delay;
    private readonly Counter<long> _dropped;
    private readonly Channel<(byte[] Data, bool Text)> _outgoing;

    public ClientSession(WebSocket socket, SubscriptionHub hub, TimeSpan delay, Counter<long> dropped)
    {
        _socket = socket;
        _hub = hub;
        _delay = delay;
        _dropped = dropped;
        _outgoing = Channel.CreateBounded<(byte[], bool)>(new BoundedChannelOptions(Capacity)
        {
            FullMode = BoundedChannelFullMode.DropOldest,
            SingleReader = true
        }, _ => _dropped.Add(1));
    }

    /// <summary>读数入队。队列满时丢最旧的（由构造时给的回调计数）。</summary>
    public void Enqueue(byte[] message) => _outgoing.Writer.TryWrite((message, false));

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        using var stop = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        // 收和发各一个循环；任何一个结束（断开、出错）都让另一个也停
        Task send = SendLoopAsync(stop.Token);
        Task receive = ReceiveLoopAsync(stop.Token);
        await Task.WhenAny(send, receive);
        await stop.CancelAsync();

        try
        {
            await Task.WhenAll(send, receive);
        }
        catch (Exception e) when (e is OperationCanceledException or WebSocketException)
        {
        }
        finally
        {
            await _hub.RemoveAsync(this);
        }
    }

    private async Task SendLoopAsync(CancellationToken cancellationToken)
    {
        await foreach ((byte[] data, bool text) in _outgoing.Reader.ReadAllAsync(cancellationToken))
        {
            await _socket.SendAsync(data, text ? WebSocketMessageType.Text : WebSocketMessageType.Binary, true, cancellationToken);

            // 仅用于演示慢客户端：连接时带 ?delayMs=2000
            if (_delay > TimeSpan.Zero)
            {
                await Task.Delay(_delay, cancellationToken);
            }
        }
    }

    private async Task ReceiveLoopAsync(CancellationToken cancellationToken)
    {
        var buffer = new byte[4096];

        while (_socket.State == WebSocketState.Open)
        {
            WebSocketReceiveResult result = await _socket.ReceiveAsync(buffer, cancellationToken);

            if (result.MessageType == WebSocketMessageType.Close)
            {
                return;
            }

            if (!result.EndOfMessage)
            {
                Reply(new { error = "message too large" });
                continue;
            }

            await HandleAsync(Encoding.UTF8.GetString(buffer, 0, result.Count));
        }
    }

    private async Task HandleAsync(string text)
    {
        Command? command;

        try
        {
            command = JsonSerializer.Deserialize<Command>(text, JsonSerializerOptions.Web);
        }
        catch (JsonException)
        {
            Reply(new { error = "not valid JSON" });
            return;
        }

        // 只接受字面通道名 Reading:<source>:<device>，不接受通配符：通配符会让一个客户端订阅所有数据
        string[] channels = command?.Channels ?? [];

        if (command?.Op is not ("subscribe" or "unsubscribe") || channels.Length is 0 or > 100 ||
            channels.Any(channel => !ChannelRule().IsMatch(channel)))
        {
            Reply(new { error = "expected {\"op\":\"subscribe|unsubscribe\",\"channels\":[\"Reading:<source>:<device>\"]}" });
            return;
        }

        foreach (string channel in channels)
        {
            await (command.Op == "subscribe" ? _hub.SubscribeAsync(this, channel) : _hub.UnsubscribeAsync(this, channel));
        }

        Reply(new { ok = command.Op, channels });
    }

    private void Reply(object body) =>
        _outgoing.Writer.TryWrite((JsonSerializer.SerializeToUtf8Bytes(body, JsonSerializerOptions.Web), true));

    [GeneratedRegex(@"^Reading:[A-Za-z0-9._-]{1,64}:[A-Za-z0-9._-]{1,64}$")]
    private static partial Regex ChannelRule();

    private sealed record Command(string? Op, string[]? Channels);
}

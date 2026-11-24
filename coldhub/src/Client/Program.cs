using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using ColdHub.Shared;

// Client：订阅 Hub 并把读数解码打印出来，用来演示"客户端怎么用 Shared 里的编解码器读二进制帧"。
// 用法：dotnet run -- <ws地址> <key> <通道,通道,...> [运行秒数]
if (args.Length < 3)
{
    Console.WriteLine("usage: dotnet run -- ws://localhost:5410/ws <api-key> Reading:warehouse-a:freezer-1[,...] [seconds]");
    return 1;
}

using var socket = new ClientWebSocket();
socket.Options.SetRequestHeader("Authorization", $"{ApiKeys.Scheme} {args[1]}");

using var stop = new CancellationTokenSource(args.Length > 3 ? TimeSpan.FromSeconds(int.Parse(args[3])) : Timeout.InfiniteTimeSpan);
Console.CancelKeyPress += (_, e) => { e.Cancel = true; stop.Cancel(); };

try
{
    await socket.ConnectAsync(new Uri(args[0]), stop.Token);
    string subscribe = JsonSerializer.Serialize(new { op = "subscribe", channels = args[2].Split(',') });
    await socket.SendAsync(Encoding.UTF8.GetBytes(subscribe), WebSocketMessageType.Text, true, stop.Token);

    var buffer = new byte[64 * 1024];

    while (socket.State == WebSocketState.Open)
    {
        WebSocketReceiveResult result = await socket.ReceiveAsync(buffer, stop.Token);

        if (result.MessageType == WebSocketMessageType.Close)
        {
            break;
        }

        if (result.MessageType == WebSocketMessageType.Text)
        {
            Console.WriteLine($"server: {Encoding.UTF8.GetString(buffer, 0, result.Count)}");
            continue;
        }

        Reading reading = ReadingCodec.Decode(buffer.AsSpan(0, result.Count));
        string probes = string.Join("  ", reading.Probes.Select(p => $"#{p.Id}={reading.Temperature(p):0.00}℃"));
        Console.WriteLine($"{UnixTime.FromNanoseconds(reading.PublishTime):HH:mm:ss.fff}  {reading.Source}/{reading.Device}  {probes}");
    }
}
catch (OperationCanceledException)
{
}
catch (WebSocketException e)
{
    Console.WriteLine($"connection failed: {e.Message} (a wrong key gives HTTP 401 before the upgrade)");
    return 2;
}

return 0;

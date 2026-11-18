// Demo 1: what a QuoteHub Source's WebSocketFeed does, in ~100 lines.
//   fake exchange (/ws)  <-- ws -->  feed client (3 connections)
// Shows: max symbols per connection, request rate limit, reconnect with backoff.
// QuoteHub counterparts: Feeds/ConnectionPlan.cs, Feeds/WebSocketFeed.cs, FeedOptions.cs
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

const int MaxSymbolsPerConnection = 2; // QuoteHub: IVenue.MaxSymbolsPerConnection
const int MaxRequestsPerSecond = 2;    // QuoteHub: IVenue.MaxRequestsPerSecond
string[] symbols = ["BTCUSD", "ETHUSD", "SOLUSD", "XRPUSD", "DOGEUSD"];

var builder = WebApplication.CreateBuilder(args);
builder.Logging.SetMinimumLevel(LogLevel.Warning); // keep the console readable
var app = builder.Build();
app.Urls.Add("http://localhost:5300");
app.UseWebSockets();

// ---- fake exchange: accepts "SUBSCRIBE A|B", answers ok, pushes a tick every 500 ms,
// ---- and kills the connection after 8 s so the client has to reconnect.
app.Map("/ws", async ctx =>
{
    if (!ctx.WebSockets.IsWebSocketRequest) { ctx.Response.StatusCode = 400; return; }
    using var ws = await ctx.WebSockets.AcceptWebSocketAsync();
    using var life = CancellationTokenSource.CreateLinkedTokenSource(ctx.RequestAborted);
    life.CancelAfter(TimeSpan.FromSeconds(8));
    var subscribed = new ConcurrentDictionary<string, byte>();
    var sendLock = new SemaphoreSlim(1); // a WebSocket allows one send at a time

    async Task Send(string text)
    {
        await sendLock.WaitAsync(life.Token);
        try { await ws.SendAsync(Encoding.UTF8.GetBytes(text), WebSocketMessageType.Text, true, life.Token); }
        finally { sendLock.Release(); }
    }

    var ticker = Task.Run(async () =>
    {
        try
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(500));
            while (await timer.WaitForNextTickAsync(life.Token))
                foreach (var s in subscribed.Keys)
                    await Send($"{{\"symbol\":\"{s}\",\"bid\":{Random.Shared.Next(100, 200)}}}");
        }
        catch (OperationCanceledException) { }
    });

    try
    {
        while (await WsText.Receive(ws, life.Token) is { } msg)
        {
            if (!msg.StartsWith("SUBSCRIBE ")) continue;
            foreach (var s in msg["SUBSCRIBE ".Length..].Split('|')) subscribed[s] = 0;
            await Send("{\"ok\":true}");
        }
    }
    catch (OperationCanceledException) { }
    catch (WebSocketException) { }
    ws.Abort(); // drop without a closing handshake, like a real network fault
    await ticker;
});

await app.StartAsync();

// ---- feed client: one task per connection, each owns a slice of the symbols.
var slices = symbols.Chunk(MaxSymbolsPerConnection).ToArray();
Console.WriteLine($"{symbols.Length} symbols / max {MaxSymbolsPerConnection} per connection => {slices.Length} connections");
var tasks = slices.Select((slice, i) => RunConnection(i, slice, app.Lifetime.ApplicationStopping)).ToArray();
await app.WaitForShutdownAsync();
await Task.WhenAll(tasks);

async Task RunConnection(int id, string[] mine, CancellationToken ct)
{
    var backoff = TimeSpan.FromSeconds(1);
    while (!ct.IsCancellationRequested)
    {
        try
        {
            using var ws = new ClientWebSocket();
            await ws.ConnectAsync(new Uri("ws://localhost:5300/ws"), ct);
            Console.WriteLine($"[conn {id}] connected, subscribing {string.Join(",", mine)}");
            // Rate limit: one SUBSCRIBE per symbol, spaced 1/MaxRequestsPerSecond apart.
            foreach (var s in mine)
            {
                await ws.SendAsync(Encoding.UTF8.GetBytes("SUBSCRIBE " + s), WebSocketMessageType.Text, true, ct);
                await Task.Delay(1000 / MaxRequestsPerSecond, ct);
            }
            while (await WsText.Receive(ws, ct) is { } msg)
            {
                Console.WriteLine($"[conn {id}] {msg}");
                backoff = TimeSpan.FromSeconds(1); // healthy again: reset the backoff
            }
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested) { return; }
        catch (Exception e) // QuoteHub logs this once at fault onset, not per retry
        {
            Console.WriteLine($"[conn {id}] lost ({e.GetType().Name}); reconnect in {backoff.TotalSeconds}s");
        }
        await Task.Delay(backoff, ct).ContinueWith(_ => { });
        backoff = TimeSpan.FromSeconds(Math.Min(backoff.TotalSeconds * 2, 5));
    }
}

static class WsText
{
    // Assumes one message per frame and < 4 KB; returns null when the peer closes.
    public static async Task<string?> Receive(WebSocket ws, CancellationToken ct)
    {
        var buf = new byte[4096];
        var r = await ws.ReceiveAsync(buf, ct);
        return r.MessageType == WebSocketMessageType.Close ? null : Encoding.UTF8.GetString(buf, 0, r.Count);
    }
}

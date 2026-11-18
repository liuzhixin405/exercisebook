// Demo 2: the Distributor's fan-out, in ~120 lines. No Valkey: an in-process Hub plays its pub/sub.
//   publisher --> Hub --> per-client bounded queue --> WebSocket client
// Shows: API-key check, subscribe/unsubscribe, one slow client cannot hurt the others (DropOldest).
// QuoteHub counterparts: QuoteHub.Distributor/Program.cs, Application/Abstractions/IApiKeys.cs
// Try: open http://localhost:5400 in two tabs; one with ?delayMs=2000 is the slow client.
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<Hub>();
builder.Services.AddHostedService<FakeSource>();
var app = builder.Build();
app.Urls.Add("http://localhost:5400");
app.UseWebSockets();

var validKeys = new HashSet<string> { "demo-key" }; // QuoteHub: hashes given by Admin

app.MapGet("/", () => Results.Content(Page.Html, "text/html"));
app.MapGet("/stats", (Hub hub) => hub.Stats());
app.MapPost("/publish/{channel}", async (string channel, HttpRequest req, Hub hub) =>
{
    hub.Publish(channel, await new StreamReader(req.Body).ReadToEndAsync());
    return Results.Ok();
});

app.Map("/ws", async (HttpContext ctx, Hub hub) =>
{
    if (!ctx.WebSockets.IsWebSocketRequest) { ctx.Response.StatusCode = 400; return; }
    if (!validKeys.Contains(ctx.Request.Query["key"].ToString())) { ctx.Response.StatusCode = 401; return; }
    var delay = int.TryParse(ctx.Request.Query["delayMs"], out var d) ? d : 0; // simulate a slow client
    using var ws = await ctx.WebSockets.AcceptWebSocketAsync();
    var client = hub.Add();
    // Sender: drains this client's own queue, so a slow socket only slows itself.
    var sender = Task.Run(async () =>
    {
        try
        {
            await foreach (var msg in client.Queue.Reader.ReadAllAsync(ctx.RequestAborted))
            {
                await ws.SendAsync(Encoding.UTF8.GetBytes(msg), WebSocketMessageType.Text, true, ctx.RequestAborted);
                if (delay > 0) await Task.Delay(delay, ctx.RequestAborted);
            }
        }
        catch (Exception) { }
    });
    try
    {
        var buf = new byte[4096];
        while (true)
        {
            var r = await ws.ReceiveAsync(buf, ctx.RequestAborted);
            if (r.MessageType == WebSocketMessageType.Close) break;
            // {"op":"subscribe"|"unsubscribe","channels":["Book:Fake:BTCUSD"]}
            using var doc = JsonDocument.Parse(buf.AsMemory(0, r.Count));
            var op = doc.RootElement.GetProperty("op").GetString();
            foreach (var ch in doc.RootElement.GetProperty("channels").EnumerateArray())
                if (op == "subscribe") hub.Subscribe(client, ch.GetString()!);
                else hub.Unsubscribe(client, ch.GetString()!);
        }
    }
    catch (Exception) { }
    hub.Remove(client);
    client.Queue.Writer.TryComplete();
    await sender;
});

app.Run();

sealed class Client
{
    // Bounded: when the client is too slow, the OLDEST message is dropped (right for market data).
    public Channel<string> Queue { get; } = Channel.CreateBounded<string>(
        new BoundedChannelOptions(10) { FullMode = BoundedChannelFullMode.DropOldest, SingleReader = true });
    public long Sent;
}

sealed class Hub
{
    readonly ConcurrentDictionary<string, ConcurrentDictionary<Client, byte>> _subs = new();
    readonly ConcurrentDictionary<Client, byte> _clients = new();
    public Client Add() { var c = new Client(); _clients[c] = 0; return c; }
    public void Remove(Client c) { _clients.TryRemove(c, out _); foreach (var s in _subs.Values) s.TryRemove(c, out _); }
    public void Subscribe(Client c, string ch) => _subs.GetOrAdd(ch, _ => new())[c] = 0;
    public void Unsubscribe(Client c, string ch) { if (_subs.TryGetValue(ch, out var s)) s.TryRemove(c, out _); }
    public void Publish(string ch, string msg)
    {
        if (!_subs.TryGetValue(ch, out var s)) return;
        foreach (var c in s.Keys) { c.Queue.Writer.TryWrite(msg); Interlocked.Increment(ref c.Sent); }
    }
    public object Stats() => new
    {
        clients = _clients.Count,
        channels = _subs.ToDictionary(kv => kv.Key, kv => kv.Value.Count),
        queued = _clients.Keys.Select(c => c.Queue.Reader.Count).ToArray(),
    };
}

sealed class FakeSource(Hub hub) : BackgroundService // plays a QuoteHub Source publishing Book:{Source}:{Symbol}
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        using var t = new PeriodicTimer(TimeSpan.FromMilliseconds(200));
        while (await t.WaitForNextTickAsync(ct))
            foreach (var sym in new[] { "BTCUSD", "ETHUSD" })
                hub.Publish($"Book:Fake:{sym}", $"{{\"ch\":\"Book:Fake:{sym}\",\"bid\":{Random.Shared.Next(100, 200)}}}");
    }
}

static class Page
{
    public const string Html = """
    <pre id=o></pre><script>
    const q = new URLSearchParams(location.search);
    const ws = new WebSocket(`ws://${location.host}/ws?key=${q.get('key') ?? 'demo-key'}&delayMs=${q.get('delayMs') ?? 0}`);
    ws.onopen = () => ws.send(JSON.stringify({op:'subscribe', channels:['Book:Fake:BTCUSD']}));
    ws.onmessage = e => { const o = document.getElementById('o'); o.textContent = e.data + '\n' + o.textContent.slice(0, 1500); };
    ws.onclose = e => document.title = 'closed ' + e.code;
    </script>
    """;
}

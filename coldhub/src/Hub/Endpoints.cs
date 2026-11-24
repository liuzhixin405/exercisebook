using System.IO.Compression;
using System.Net.WebSockets;
using System.Text;
using System.Diagnostics.Metrics;
using ColdHub.Shared;
using Microsoft.Extensions.Options;

namespace ColdHub.Hub;

public static class Endpoints
{
    /// <summary>
    /// 实时订阅。浏览器的 WebSocket 不能设请求头，所以也接受 ?key=（key 会出现在 URL 里，会被访问日志记下，生产中要想清楚）。校验通过才升级成 WebSocket，否则直接 401。
    /// </summary>
    public static void MapSubscribe(this WebApplication app)
    {
        Counter<long> dropped = app.Services.GetRequiredService<IMeterFactory>().Create("ColdHub.Hub")
            .CreateCounter<long>("coldhub.hub.messages.dropped", "{message}", "Messages dropped because a client was too slow.");
        dropped.Add(0);
        int clients = 0;
        app.Services.GetRequiredService<IMeterFactory>().Create("ColdHub.Hub")
            .CreateObservableGauge("coldhub.hub.clients", () => Volatile.Read(ref clients), "{client}", "Connected clients.");

        app.Map("/ws", async (HttpContext context, KeyDirectory keys, SubscriptionHub hub) =>
        {
            string? key = ApiKeys.FromHeader(context.Request.Headers.Authorization) ?? context.Request.Query["key"];

            if (!keys.IsValid(key))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            if (!context.WebSockets.IsWebSocketRequest)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            _ = int.TryParse(context.Request.Query["delayMs"], out int delayMs);
            using WebSocket socket = await context.WebSockets.AcceptWebSocketAsync();
            var session = new ClientSession(socket, hub, TimeSpan.FromMilliseconds(Math.Clamp(delayMs, 0, 5000)), dropped);

            Interlocked.Increment(ref clients);

            try
            {
                await session.RunAsync(context.RequestAborted);
            }
            finally
            {
                Interlocked.Decrement(ref clients);
            }
        });

        app.MapGet("/stats", (SubscriptionHub hub, KeyDirectory keys, HttpRequest request) =>
            keys.IsValidRequest(request) ? Results.Ok(new { topics = hub.Topics }) : Results.Unauthorized());
    }

    /// <summary>
    /// 历史下载：GET /history/{source}/{date}?format=csv.gz&amp;devices=a,b。
    /// 不带 devices 直接把文件原样发出去（它本来就是 .gz，不用解压）；带了就解压、按设备过滤、再压。
    /// </summary>
    public static void MapHistory(this WebApplication app)
    {
        app.MapGet("/history/{source}/{date}", async (string source, string date, string? format, string? devices,
            HttpRequest request, HttpResponse response, KeyDirectory keys, IOptions<HubOptions> options) =>
        {
            if (!keys.IsValidRequest(request))
            {
                return Results.Unauthorized();
            }

            // 这三个参数会拼进文件路径，所以必须先校验：名字用白名单字符，日期严格解析，杜绝 ../ 逃出历史目录
            if (!Channels.IsValidName(source) ||
                !DateOnly.TryParseExact(date, "yyyy-MM-dd", out DateOnly day) ||
                format is not (null or "csv.gz"))
            {
                return Results.BadRequest("source: a registered name; date: yyyy-MM-dd; format: csv.gz");
            }

            string[]? wanted = devices?.Split(',', StringSplitOptions.RemoveEmptyEntries);

            if (wanted is not null && (wanted.Length == 0 || wanted.Any(d => !Channels.IsValidName(d))))
            {
                return Results.BadRequest("devices: comma separated device names");
            }

            string path = Path.GetFullPath(Path.Combine(options.Value.HistoryDir, source, $"{day:yyyy-MM-dd}.csv.gz"));

            if (!File.Exists(path))
            {
                return Results.NotFound("no history of that source on that day");
            }

            // Recorder 可能正在追加：以允许共享写的方式打开
            var file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
            string name = $"{source}-{day:yyyy-MM-dd}.csv.gz";

            if (wanted is null)
            {
                return Results.File(file, "application/gzip", name);
            }

            response.ContentType = "application/gzip";
            response.Headers.ContentDisposition = $"attachment; filename=\"{name}\"";

            await using (file)
            {
                // 注意：Kestrel 不许同步写响应。StreamWriter / GZipStream 的 Dispose 会同步写出最后的数据，
                // 所以它们都用 leaveOpen，并显式 Flush / DisposeAsync，保证收尾也是异步的
                var output = new GZipStream(response.Body, CompressionLevel.Fastest, leaveOpen: true);
                var writer = new StreamWriter(output, new UTF8Encoding(false), 4096, leaveOpen: true) { NewLine = "\n" };

                try
                {
                    // GZipStream 会把首尾相连的多个 gzip 成员当作一个流读完
                    using var reader = new StreamReader(new GZipStream(file, CompressionMode.Decompress));
                    string? line = await reader.ReadLineAsync();

                    while (line is not null)
                    {
                        // 第一行是表头；数据行的第二列是 device，形如 "src","dev",...
                        if (line.StartsWith("\"source\"", StringComparison.Ordinal) || wanted.Contains(DeviceOf(line)))
                        {
                            await writer.WriteLineAsync(line);
                        }

                        line = await reader.ReadLineAsync();
                    }

                    await writer.FlushAsync();
                }
                finally
                {
                    await output.DisposeAsync();
                }
            }

            return Results.Empty;
        });
    }

    private static string DeviceOf(string row) => row.Split(',', 3)[1].Trim('"');
}

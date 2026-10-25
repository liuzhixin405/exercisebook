using System.Net;
using System.Text;
using System.Text.Json;

namespace AiDevPlatform.Web.Services;

// [来源: aspnet-core-aspnetcore-10.0#0247 p1644-1647]
public sealed class DevelopmentApiClient
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

    public DevelopmentApiClient(HttpClient client) { _client = client; }

    public async Task<T> GetAsync<T>(string route, CancellationToken cancellationToken)
    {
        using var response = await _client.GetAsync("api/development/" + route, cancellationToken);
        response.EnsureSuccessStatusCode();
        return JsonSerializer.Deserialize<T>(await response.Content.ReadAsStringAsync(cancellationToken), Options)
            ?? throw new JsonException("API 返回空内容。");
    }

    public async Task<JsonElement> PostAsync(string route, object input, CancellationToken cancellationToken)
    {
        using var body = new StringContent(JsonSerializer.Serialize(input, Options), Encoding.UTF8, "application/json");
        using var response = await _client.PostAsync("api/development/" + route, body, cancellationToken);
        response.EnsureSuccessStatusCode();
        return JsonSerializer.Deserialize<JsonElement>(await response.Content.ReadAsStringAsync(cancellationToken));
    }

    public async Task<byte[]> DownloadAsync(Guid id, Guid changeId, CancellationToken cancellationToken)
    {
        using var response = await _client.GetAsync($"api/development/tasks/{id:D}/changes/{changeId:D}/download", cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync(cancellationToken);
    }

    public static bool IsLocal(HttpContext context, IWebHostEnvironment environment) => environment.IsDevelopment() &&
        context.Connection.RemoteIpAddress is { } address && IPAddress.IsLoopback(address) &&
        context.Request.Host.Host is "localhost" or "127.0.0.1" or "::1" or "[::1]";
}
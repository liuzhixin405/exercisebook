using System.Text;
using System.Text.Json;
using AiDevPlatform.Shared.DTOs.Application;

namespace AiDevPlatform.Web.Services;

public sealed class ApplicationsApiClient
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public ApplicationsApiClient(HttpClient client)
    {
        _client = client;
    }

    public Task<List<ApplicationSummaryDto>> ListAsync(CancellationToken cancellationToken) =>
        SendAsync<List<ApplicationSummaryDto>>(HttpMethod.Get, "api/applications", null, cancellationToken);

    public Task<ApplicationSummaryDto> GetAsync(Guid id, CancellationToken cancellationToken) =>
        SendAsync<ApplicationSummaryDto>(HttpMethod.Get, $"api/applications/{id:D}", null, cancellationToken);

    public Task<ApplicationSummaryDto> SaveAsync(Guid? id, SaveApplicationRequest input, CancellationToken cancellationToken) =>
        SendAsync<ApplicationSummaryDto>(id.HasValue ? HttpMethod.Put : HttpMethod.Post,
            id.HasValue ? $"api/applications/{id.Value:D}" : "api/applications", input, cancellationToken);

    // [来源: aspnet-core-aspnetcore-10.0#0247 p1641, p1647]
    private async Task<T> SendAsync<T>(HttpMethod method, string route, SaveApplicationRequest? input,
        CancellationToken cancellationToken) where T : class
    {
        using var request = new HttpRequestMessage(method, route);
        if (input != null)
            request.Content = new StringContent(JsonSerializer.Serialize(input, JsonOptions), Encoding.UTF8, "application/json");
        using var response = await _client.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        // [联网-官方: https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/deserialization]
        return JsonSerializer.Deserialize<T>(await response.Content.ReadAsStringAsync(cancellationToken), JsonOptions)
            ?? throw new JsonException("API 未返回应用数据。");
    }
}
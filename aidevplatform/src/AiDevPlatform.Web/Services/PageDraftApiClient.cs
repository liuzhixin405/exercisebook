using System.Text;
using System.Text.Json;
using AiDevPlatform.Shared.DTOs.ApplicationPage;

namespace AiDevPlatform.Web.Services;

public sealed class PageDraftApiClient
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public PageDraftApiClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<PageDraft> GenerateAsync(string prompt, CancellationToken cancellationToken)
    {
        // [来源: aspnet-core-aspnetcore-10.0#0247 p1647]
        using var content = new StringContent(
            JsonSerializer.Serialize(new GeneratePageDraftRequest { Prompt = prompt }, JsonOptions),
            Encoding.UTF8, "application/json");
        using var response = await _client.PostAsync("api/ai/page-drafts", content, cancellationToken);
        response.EnsureSuccessStatusCode();

        // [联网-官方: https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/deserialization]
        return JsonSerializer.Deserialize<PageDraft>(
            await response.Content.ReadAsStringAsync(cancellationToken), JsonOptions)
            ?? throw new JsonException("API 未返回页面草稿。");
    }
}
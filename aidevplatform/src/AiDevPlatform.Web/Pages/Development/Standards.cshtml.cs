using System.Text.Json;
using AiDevPlatform.Shared.DTOs.Application;
using AiDevPlatform.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace AiDevPlatform.Web.Pages.Development;

[RequestSizeLimit(32768)]
public sealed class StandardsModel : DevelopmentPageModel
{
    public StandardsModel(DevelopmentApiClient client, IWebHostEnvironment environment) : base(client, environment) { }
    public List<TeamStandard> Standards { get; private set; } = new();
    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        if (!IsLocal) return StatusCode(403);
        try { Standards = await Client.GetAsync<List<TeamStandard>>("standards", cancellationToken); }
        catch (Exception exception) when (exception is HttpRequestException or JsonException or OperationCanceledException)
        { Response.StatusCode = 502; ErrorMessage = "规范加载失败，请检查 API 服务后重试。"; }
        return Page();
    }
    public Task<IActionResult> OnPostAsync([FromBody] TeamStandard input, CancellationToken cancellationToken) =>
        ForwardAsync("standards", input, cancellationToken);
}
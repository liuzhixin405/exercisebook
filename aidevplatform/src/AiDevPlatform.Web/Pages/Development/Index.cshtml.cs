using System.Text.Json;
using AiDevPlatform.Shared.DTOs.Application;
using AiDevPlatform.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace AiDevPlatform.Web.Pages.Development;

// [联网-官方: https://learn.microsoft.com/en-us/aspnet/core/razor-pages/?view=aspnetcore-8.0]
[RequestSizeLimit(262144)]
public sealed class IndexModel : DevelopmentPageModel
{
    public IndexModel(DevelopmentApiClient client, IWebHostEnvironment environment) : base(client, environment) { }
    public List<TeamStandard> Standards { get; private set; } = new();
    public List<DevelopmentTask> Tasks { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        if (!IsLocal) return StatusCode(403);
        try
        {
            Standards = await Client.GetAsync<List<TeamStandard>>("standards", cancellationToken);
            Tasks = await Client.GetAsync<List<DevelopmentTask>>("tasks", cancellationToken);
        }
        catch (Exception exception) when (exception is HttpRequestException or JsonException or OperationCanceledException)
        { Response.StatusCode = 502; ErrorMessage = "工作台加载失败，请检查 API 服务后重试。"; }
        return Page();
    }

    public Task<IActionResult> OnPostAsync([FromBody] CreateDevelopmentTask input, CancellationToken cancellationToken) =>
        ForwardAsync("tasks", input, cancellationToken);
}
using System.Text.Json;
using AiDevPlatform.Shared.DTOs.Application;
using AiDevPlatform.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace AiDevPlatform.Web.Pages.Development;

[RequestSizeLimit(65536)]
public sealed class TaskModel : DevelopmentPageModel
{
    public TaskModel(DevelopmentApiClient client, IWebHostEnvironment environment) : base(client, environment) { }
    public DevelopmentTask? Development { get; private set; }

    public async Task<IActionResult> OnGetAsync(Guid id, CancellationToken cancellationToken)
    {
        if (!IsLocal) return StatusCode(403);
        try { Development = await Client.GetAsync<DevelopmentTask>($"tasks/{id:D}", cancellationToken); }
        catch (HttpRequestException exception) when (exception.StatusCode == System.Net.HttpStatusCode.NotFound) { return NotFound(); }
        catch (Exception exception) when (exception is HttpRequestException or JsonException or OperationCanceledException)
        { Response.StatusCode = 502; ErrorMessage = "任务加载失败，请检查 API 服务后重试。"; }
        return Page();
    }

    public Task<IActionResult> OnPostAsync(Guid id, string? operation, [FromBody] JsonElement input, CancellationToken cancellationToken) =>
        operation is "clarify" or "confirm" or "generate" or "review" or "verify" or "materialize"
            ? ForwardAsync($"tasks/{id:D}/{operation}", input, cancellationToken)
            : System.Threading.Tasks.Task.FromResult<IActionResult>(BadRequest());

    public async Task<IActionResult> OnGetDownloadAsync(Guid id, Guid changeId, CancellationToken cancellationToken)
    {
        if (!IsLocal) return StatusCode(403);
        try { return File(await Client.DownloadAsync(id, changeId, cancellationToken), "application/zip", $"delivery-{changeId:N}.zip"); }
        catch (Exception exception) when (exception is HttpRequestException or JsonException or OperationCanceledException)
        { return Failure(exception); }
    }
}
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Text.Json;
using AiDevPlatform.Core.Features.Applications;
using AiDevPlatform.Core.Interfaces;
using AiDevPlatform.Shared.DTOs.Application;
using Microsoft.AspNetCore.Mvc;

namespace AiDevPlatform.API.Controllers;

// [来源: aspnet-core-aspnetcore-10.0#0554 p4650-4658]
[ApiController]
[Route("api/development")]
[RequestSizeLimit(262144)]
public sealed class DevelopmentController : ControllerBase
{
    private readonly DevelopmentWorkflow _workflow;
    private readonly IDevelopmentStore _store;
    private readonly IWebHostEnvironment _environment;

    public DevelopmentController(DevelopmentWorkflow workflow, IDevelopmentStore store, IWebHostEnvironment environment)
    {
        _workflow = workflow; _store = store; _environment = environment;
    }

    [HttpGet("standards")]
    public Task<IActionResult> Standards(CancellationToken cancellationToken) => Execute(async () =>
        Ok(await _store.StandardsAsync(cancellationToken)));

    [HttpPost("standards")]
    [Consumes("application/json")]
    public Task<IActionResult> AddStandard([FromBody] TeamStandard input, CancellationToken cancellationToken) => Execute(async () =>
        Ok(await _workflow.AddStandardAsync(input, cancellationToken)));

    [HttpGet("tasks")]
    public Task<IActionResult> Tasks(CancellationToken cancellationToken) => Execute(async () =>
        Ok((await _store.TasksAsync(cancellationToken)).OrderByDescending(task => task.CreatedAt).Select(task => new DevelopmentTask
        {
            Id = task.Id, Revision = task.Revision, Title = task.Title, Technology = task.Technology,
            Standard = new TeamStandard { Id = task.Standard.Id, Name = task.Standard.Name, Version = task.Standard.Version },
            Confirmed = task.Confirmed, CreatedAt = task.CreatedAt,
            Changes = task.Changes.Select(change => new ChangeRevision { Id = change.Id, Review = change.Review }).ToList()
        })));

    [HttpPost("tasks")]
    [Consumes("application/json")]
    public Task<IActionResult> Create([FromBody] CreateDevelopmentTask input, CancellationToken cancellationToken) => Execute(async () =>
        Ok(await _workflow.CreateAsync(input, cancellationToken)));

    [HttpGet("tasks/{id:guid}")]
    public Task<IActionResult> Get(Guid id, CancellationToken cancellationToken) => Execute(async () =>
        Ok(await _workflow.GetAsync(id, cancellationToken)));

    [HttpPost("tasks/{id:guid}/clarify")]
    [Consumes("application/json")]
    public Task<IActionResult> Clarify(Guid id, [FromBody] DevelopmentAction input, CancellationToken cancellationToken) => Execute(async () =>
        Ok(await _workflow.ClarifyAsync(id, input, cancellationToken)));

    [HttpPost("tasks/{id:guid}/confirm")]
    [Consumes("application/json")]
    public Task<IActionResult> Confirm(Guid id, [FromBody] ConfirmDevelopmentTask input, CancellationToken cancellationToken) => Execute(async () =>
        Ok(await _workflow.ConfirmAsync(id, input, cancellationToken)));

    [HttpPost("tasks/{id:guid}/generate")]
    [Consumes("application/json")]
    public Task<IActionResult> Generate(Guid id, [FromBody] DevelopmentAction input, CancellationToken cancellationToken) => Execute(async () =>
        Ok(await _workflow.GenerateAsync(id, input, cancellationToken)));

    [HttpPost("tasks/{id:guid}/review")]
    [Consumes("application/json")]
    public Task<IActionResult> Review(Guid id, [FromBody] DevelopmentAction input, CancellationToken cancellationToken) => Execute(async () =>
        Ok(await _workflow.ReviewAsync(id, input, cancellationToken)));

    [HttpPost("tasks/{id:guid}/materialize")]
    [Consumes("application/json")]
    public Task<IActionResult> Materialize(Guid id, [FromBody] DevelopmentAction input, CancellationToken cancellationToken) => Execute(async () =>
        Ok(await _workflow.MaterializeAsync(id, input, cancellationToken)));

    [HttpPost("tasks/{id:guid}/verify")]
    [Consumes("application/json")]
    public Task<IActionResult> Verify(Guid id, [FromBody] DevelopmentAction input, CancellationToken cancellationToken) => Execute(async () =>
        Ok(await _workflow.VerifyAsync(id, input, cancellationToken)));

    [HttpGet("tasks/{id:guid}/changes/{changeId:guid}/download")]
    public Task<IActionResult> Download(Guid id, Guid changeId, CancellationToken cancellationToken) => Execute(async () =>
    {
        var task = await _workflow.GetAsync(id, cancellationToken);
        var change = task.Changes.FirstOrDefault(item => item.Id == changeId) ?? throw new KeyNotFoundException();
        if (change.Review != "accepted" || !DevelopmentWorkflow.CanDeliver(task.Standard, change))
            return Conflict(new { error = "该版本尚未通过人工审阅及硬性检查，不能导出交付包。" });
        DevelopmentWorkflow.ValidateFiles(change.Files, 24, 120000, false);
        // [联网-官方: https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.ziparchive?view=net-8.0]
        using var output = new MemoryStream();
        using (var archive = new ZipArchive(output, ZipArchiveMode.Create, true))
        {
            foreach (var file in change.Files)
            {
                using var writer = new StreamWriter(archive.CreateEntry(file.Path).Open(), new UTF8Encoding(false));
                await writer.WriteAsync(file.Content.AsMemory(), cancellationToken);
            }
            using var metadata = new StreamWriter(archive.CreateEntry("_platform/delivery.json").Open(), new UTF8Encoding(false));
            await metadata.WriteAsync(JsonSerializer.Serialize(new { taskId = task.Id, task.Title, task.Technology, task.Requirement,
                task.Standard, task.Draft, task.Answers, changeId = change.Id, change.CreatedAt, change.Generation,
                change.GenerationTargetPath, change.GenerationFeedback, change.Checks, change.Verifications, change.Review, change.ReviewComment },
                new JsonSerializerOptions(JsonSerializerDefaults.Web)).AsMemory(), cancellationToken);
        }
        return File(output.ToArray(), "application/zip", $"delivery-{change.Id:N}.zip");
    });

    private async Task<IActionResult> Execute(Func<Task<IActionResult>> action)
    {
        if (!_environment.IsDevelopment() || HttpContext.Connection.RemoteIpAddress is not { } address ||
            !IPAddress.IsLoopback(address) || Request.Host.Host is not ("localhost" or "127.0.0.1" or "::1" or "[::1]") ||
            Request.Headers.ContainsKey("Origin") || Request.Headers.ContainsKey("Sec-Fetch-Site") ||
            Request.Headers["X-LowCode-Client"] != "Web") return StatusCode(403);
        try { return await action(); }
        catch (ArgumentException) { return BadRequest(new { error = "输入无效，请检查必填项、长度和文件路径。" }); }
        catch (KeyNotFoundException) { return NotFound(); }
        catch (InvalidOperationException) { return Conflict(new { error = "状态或版本冲突，请刷新并检查流程前置条件；模型也可能忙碌。" }); }
        catch (OperationCanceledException) { return StatusCode(504, new { error = "操作已取消或超时，刷新确认状态后重试。" }); }
        catch (NotSupportedException) { return StatusCode(503, new { error = "未配置可信的隔离验证器，必需检查不能跳过。" }); }
        catch (JsonException exception)
        {
            var (code, error) = exception.Message switch
            {
                "模型输出未完整结束。" => ("model-output-truncated", "模型输出被截断，本批未保存。请缩小本批内容或调整模型输出预算后重试。"),
                "模型没有严格返回本批计划的完整文件，已有版本未改变。" => ("model-batch-mismatch", "模型未严格返回当前批次指定的完整文件，本批未保存；已有版本未改变。"),
                "AI 文件不满足安全边界。" => ("model-source-rejected", "模型返回文件未通过路径、大小或内容边界检查，本批未保存。"),
                _ => ("invalid-upstream-json", "模型、验证器响应或存储内容无效，操作未确认成功。")
            };
            return StatusCode(502, new { error, code });
        }
        catch (HttpRequestException)
        { return StatusCode(502, new { error = "模型、验证器响应或存储内容无效，操作未确认成功。" }); }
        catch (IOException) { return StatusCode(503, new { error = "本机工作区暂不可用。" }); }
    }
}
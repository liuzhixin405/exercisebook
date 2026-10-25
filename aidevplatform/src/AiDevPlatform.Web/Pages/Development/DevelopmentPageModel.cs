using System.Text.Json;
using AiDevPlatform.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AiDevPlatform.Web.Pages.Development;

public abstract class DevelopmentPageModel : PageModel
{
    protected readonly DevelopmentApiClient Client;
    private readonly IWebHostEnvironment _environment;
    protected DevelopmentPageModel(DevelopmentApiClient client, IWebHostEnvironment environment)
    { Client = client; _environment = environment; }
    protected bool IsLocal => DevelopmentApiClient.IsLocal(HttpContext, _environment);
    public string? ErrorMessage { get; protected set; }

    protected async Task<IActionResult> ForwardAsync(string route, object input, CancellationToken cancellationToken)
    {
        if (!IsLocal) return StatusCode(403);
        try { return new JsonResult(await Client.PostAsync(route, input, cancellationToken)); }
        catch (Exception exception) when (exception is HttpRequestException or JsonException or OperationCanceledException)
        { return Failure(exception, route.EndsWith("/verify", StringComparison.Ordinal)); }
    }

    protected IActionResult Failure(Exception exception, bool verification = false)
    {
        var status = exception is OperationCanceledException ? 504 : exception is HttpRequestException http &&
            http.StatusCode is { } code && (int)code is 400 or 403 or 404 or 409 or 503 or 504 ? (int)code : 502;
        var message = status switch
        {
            400 => "输入无效，请检查长度、路径、需求稿和所有问题的回答。",
            403 => "该工作区仅供本机开发环境使用。",
            404 => "任务或源码版本不存在。",
            409 => "状态或版本冲突：请刷新，确认前置步骤、规范检查及审阅风险；模型也可能忙碌。",
            504 => "操作取消或超时，请刷新确认状态后重试。",
            _ => "API、模型或工作区暂不可用，未确认操作成功。"
        };
        if (verification && status is 502 or 503)
            message = "隔离验证器未配置、不可用或报告无效；必需检查不能跳过，未确认验证通过。";
        return StatusCode(status, new { error = message });
    }
}
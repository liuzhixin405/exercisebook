using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Text.Json;
using AiDevPlatform.Web.Services;

namespace AiDevPlatform.Web.Pages.Designer;

[RequestSizeLimit(16384)]
public class IndexModel : PageModel
{
    private readonly PageDraftApiClient _generator;
    private readonly ApplicationsApiClient _applications;
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;

    public IndexModel(PageDraftApiClient generator, ApplicationsApiClient applications,
        IWebHostEnvironment environment, IConfiguration configuration)
    {
        _generator = generator;
        _applications = applications;
        _environment = environment;
        _configuration = configuration;
    }

    public string? ApplicationId { get; set; }
    public string? PageId { get; set; }
    public string? ApplicationName { get; private set; }

    // [联网-官方: https://learn.microsoft.com/en-us/dotnet/api/system.net.ipaddress.isloopback]
    private bool IsLocalDevelopment => _environment.IsDevelopment() &&
        HttpContext.Connection.RemoteIpAddress is { } address && IPAddress.IsLoopback(address) &&
        Request.Host.Host is "localhost" or "127.0.0.1" or "::1" or "[::1]";

    public bool AiAvailable => IsLocalDevelopment && _configuration["UiFeatures:AiDraftEnabled"] == "true";

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken, string? applicationId = null,
        string? pageId = null)
    {
        PageId = pageId;
        if (string.IsNullOrEmpty(applicationId)) return Page();
        if (!IsLocalDevelopment) return StatusCode(403);
        if (!Guid.TryParse(applicationId, out var id)) return NotFound();
        try
        {
            var application = await _applications.GetAsync(id, cancellationToken);
            ApplicationId = application.Id.ToString("D");
            ApplicationName = application.Name;
        }
        catch (HttpRequestException exception) when (exception.StatusCode == HttpStatusCode.NotFound)
        {
            return NotFound();
        }
        catch (Exception exception) when (exception is HttpRequestException or JsonException or OperationCanceledException)
        {
            return StatusCode(502, "无法加载应用，请检查 API 服务后重试。");
        }
        return Page();
    }

    // [联网-官方: https://learn.microsoft.com/en-us/aspnet/core/security/anti-request-forgery?view=aspnetcore-8.0]
    public async Task<IActionResult> OnPostGenerateAsync(string? prompt, CancellationToken cancellationToken)
    {
        if (!AiAvailable)
            return new JsonResult(new { error = "AI 生成仅向本机开发环境开放。" }) { StatusCode = 403 };
        if (!ModelState.IsValid || string.IsNullOrWhiteSpace(prompt) || prompt.Length > 2000)
            return new JsonResult(new { error = "需求描述须为 1 至 2000 个字符。" }) { StatusCode = 400 };

        try
        {
            return new JsonResult(await _generator.GenerateAsync(prompt, cancellationToken));
        }
        catch (ArgumentException)
        {
            return new JsonResult(new { error = "需求或模型配置无效。" }) { StatusCode = 400 };
        }
        catch (InvalidOperationException)
        {
            return new JsonResult(new { error = "本地 AI 忙碌或配置不可用，请稍后重试。" }) { StatusCode = 409 };
        }
        catch (OperationCanceledException)
        {
            return new JsonResult(new { error = "生成已取消或请求超时，请重试。" }) { StatusCode = 504 };
        }
        catch (HttpRequestException exception) when (exception.StatusCode is
            HttpStatusCode.BadRequest or HttpStatusCode.Forbidden or HttpStatusCode.Conflict or HttpStatusCode.GatewayTimeout)
        {
            var message = exception.StatusCode switch
            {
                HttpStatusCode.BadRequest => "需求描述无效，请修改后重试。",
                HttpStatusCode.Forbidden => "API 未开放本机 AI 生成，请检查 API 开发配置。",
                HttpStatusCode.Conflict => "本地 AI 忙碌或配置不可用，请稍后重试。",
                _ => "模型生成已超时，请稍后重试。"
            };
            return new JsonResult(new { error = message }) { StatusCode = (int)exception.StatusCode.Value };
        }
        catch (Exception exception) when (exception is HttpRequestException or JsonException or KeyNotFoundException or IndexOutOfRangeException)
        {
            return new JsonResult(new { error = "API 或模型不可用，或响应无效，请检查两个服务后重试。" }) { StatusCode = 502 };
        }
    }
}

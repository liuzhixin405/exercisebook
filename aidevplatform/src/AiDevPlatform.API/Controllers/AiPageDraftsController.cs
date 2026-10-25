using System.Net;
using System.Text.Json;
using AiDevPlatform.Core.Interfaces;
using AiDevPlatform.Shared.DTOs.ApplicationPage;
using Microsoft.AspNetCore.Mvc;

namespace AiDevPlatform.API.Controllers;

// [来源: aspnet-core-aspnetcore-10.0#0554 p4650-4658]
[ApiController]
[Route("api/ai/page-drafts")]
[RequestSizeLimit(16384)]
public sealed class AiPageDraftsController : ControllerBase
{
    private readonly IPageDraftGenerator _generator;
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;

    public AiPageDraftsController(IPageDraftGenerator generator, IWebHostEnvironment environment,
        IConfiguration configuration)
    {
        _generator = generator;
        _environment = environment;
        _configuration = configuration;
    }

    [HttpPost]
    [Consumes("application/json")]
    public async Task<IActionResult> Generate([FromBody] GeneratePageDraftRequest request,
        CancellationToken cancellationToken)
    {
        // [联网-官方: https://learn.microsoft.com/en-us/dotnet/api/system.net.ipaddress.isloopback?view=net-8.0]
        if (!_environment.IsDevelopment() || _configuration["LocalAI:Enabled"] != "true" ||
            HttpContext.Connection.RemoteIpAddress is not { } address || !IPAddress.IsLoopback(address) ||
            Request.Host.Host is not ("localhost" or "127.0.0.1" or "::1" or "[::1]") ||
            Request.Headers.ContainsKey("Origin") || Request.Headers.ContainsKey("Sec-Fetch-Site") ||
            Request.Headers["X-LowCode-Client"] != "Web")
            return StatusCode(403, new { error = "AI 接口仅供本机开发环境的 Web 服务调用。" });

        if (string.IsNullOrWhiteSpace(request.Prompt) || request.Prompt.Length > 2000)
            return BadRequest(new { error = "需求描述须为 1 至 2000 个字符。" });

        try
        {
            return Ok(await _generator.GenerateAsync(request.Prompt, cancellationToken));
        }
        catch (ArgumentException)
        {
            return BadRequest(new { error = "需求或模型配置无效。" });
        }
        catch (InvalidOperationException)
        {
            return Conflict(new { error = "本地 AI 忙碌或配置不可用，请稍后重试。" });
        }
        catch (OperationCanceledException)
        {
            return StatusCode(504, new { error = "生成已取消或超过 90 秒，请重试。" });
        }
        catch (Exception exception) when (exception is HttpRequestException or JsonException or
            KeyNotFoundException or IndexOutOfRangeException)
        {
            return StatusCode(502, new { error = "模型离线或输出无效，请检查本地模型后重试。" });
        }
    }
}
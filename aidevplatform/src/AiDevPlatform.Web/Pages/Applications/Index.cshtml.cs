using System.Net;
using System.Text.Json;
using AiDevPlatform.Shared.DTOs.Application;
using AiDevPlatform.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AiDevPlatform.Web.Pages.Applications;

// [联网-官方: https://learn.microsoft.com/en-us/aspnet/core/razor-pages/?view=aspnetcore-8.0]
public sealed class IndexModel : PageModel
{
    private readonly ApplicationsApiClient _applications;
    private readonly IWebHostEnvironment _environment;

    public IndexModel(ApplicationsApiClient applications, IWebHostEnvironment environment)
    {
        _applications = applications;
        _environment = environment;
    }

    public List<ApplicationSummaryDto> Applications { get; private set; } = new();
    public string SearchTerm { get; private set; } = string.Empty;
    public string? ErrorMessage { get; private set; }

    public async Task<IActionResult> OnGetAsync(string? searchTerm, CancellationToken cancellationToken)
    {
        if (!_environment.IsDevelopment() || HttpContext.Connection.RemoteIpAddress is not { } address ||
            !IPAddress.IsLoopback(address) || Request.Host.Host is not ("localhost" or "127.0.0.1" or "::1" or "[::1]"))
            return StatusCode(403);
        if (searchTerm?.Length > 100) return BadRequest();
        SearchTerm = searchTerm?.Trim() ?? string.Empty;
        try
        {
            Applications = await _applications.ListAsync(cancellationToken);
            if (SearchTerm.Length > 0)
                Applications = Applications.Where(application =>
                    application.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    application.Code.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
        }
        catch (Exception exception) when (exception is HttpRequestException or JsonException or OperationCanceledException)
        {
            Response.StatusCode = 502;
            ErrorMessage = "无法加载应用，请确认 API 服务正在运行后重试。";
        }
        return Page();
    }
}
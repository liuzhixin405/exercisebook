using System.Net;
using System.Text.Json;
using AiDevPlatform.Shared.DTOs.Application;
using AiDevPlatform.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AiDevPlatform.Web.Pages.Applications;

// [联网-官方: https://learn.microsoft.com/en-us/aspnet/core/razor-pages/?view=aspnetcore-8.0]
[RequestSizeLimit(16384)]
public sealed class EditModel : PageModel
{
    private readonly ApplicationsApiClient _applications;
    private readonly IWebHostEnvironment _environment;

    public EditModel(ApplicationsApiClient applications, IWebHostEnvironment environment)
    {
        _applications = applications;
        _environment = environment;
    }

    [BindProperty]
    public SaveApplicationRequest Input { get; set; } = new();
    public Guid? ApplicationId { get; private set; }
    public string? LoadError { get; private set; }

    private bool IsLocalDevelopment => _environment.IsDevelopment() &&
        HttpContext.Connection.RemoteIpAddress is { } address && IPAddress.IsLoopback(address) &&
        Request.Host.Host is "localhost" or "127.0.0.1" or "::1" or "[::1]";

    public async Task<IActionResult> OnGetAsync(Guid? id, CancellationToken cancellationToken)
    {
        if (!IsLocalDevelopment) return StatusCode(403);
        ApplicationId = id;
        if (!id.HasValue) return Page();
        try
        {
            var application = await _applications.GetAsync(id.Value, cancellationToken);
            Input = new SaveApplicationRequest
            {
                Name = application.Name, Code = application.Code,
                Description = application.Description, Version = application.Version
            };
        }
        catch (HttpRequestException exception) when (exception.StatusCode == HttpStatusCode.NotFound)
        {
            return NotFound();
        }
        catch (Exception exception) when (exception is HttpRequestException or JsonException or OperationCanceledException)
        {
            Response.StatusCode = 502;
            LoadError = "无法读取应用，请确认 API 服务正在运行后重试。";
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(Guid? id, CancellationToken cancellationToken)
    {
        if (!IsLocalDevelopment) return StatusCode(403);
        ApplicationId = id;
        if (!ModelState.IsValid)
        {
            Response.StatusCode = 400;
            return Page();
        }
        try
        {
            await _applications.SaveAsync(id, Input, cancellationToken);
            TempData["ApplicationMessage"] = id.HasValue ? "应用已更新。" : "应用已创建。";
            return RedirectToPage("./Index");
        }
        catch (HttpRequestException exception) when (exception.StatusCode == HttpStatusCode.NotFound)
        {
            return NotFound();
        }
        catch (HttpRequestException exception) when (exception.StatusCode == HttpStatusCode.Conflict)
        {
            Response.StatusCode = 409;
            ModelState.AddModelError("Input.Code", "应用编码已存在，请使用其他编码。");
        }
        catch (Exception exception) when (exception is HttpRequestException or JsonException or OperationCanceledException)
        {
            Response.StatusCode = 502;
            ModelState.AddModelError(string.Empty, "保存失败，请检查 API 服务后重试。未收到保存确认。");
        }
        return Page();
    }
}
using System.ComponentModel.DataAnnotations;
using System.Net;
using AiDevPlatform.Core.Features.Applications;
using AiDevPlatform.Shared.DTOs.Application;
using Microsoft.AspNetCore.Mvc;

namespace AiDevPlatform.API.Controllers;

// [来源: aspnet-core-aspnetcore-10.0#0554 p4657-4663]
[ApiController]
[Route("api/applications")]
[RequestSizeLimit(16384)]
public sealed class ApplicationsController : ControllerBase
{
    private readonly ApplicationManagementService _applications;
    private readonly IWebHostEnvironment _environment;

    public ApplicationsController(ApplicationManagementService applications, IWebHostEnvironment environment)
    {
        _applications = applications;
        _environment = environment;
    }

    private bool IsLocalWebRequest => _environment.IsDevelopment() &&
        HttpContext.Connection.RemoteIpAddress is { } address && IPAddress.IsLoopback(address) &&
        Request.Host.Host is "localhost" or "127.0.0.1" or "::1" or "[::1]" &&
        !Request.Headers.ContainsKey("Origin") && !Request.Headers.ContainsKey("Sec-Fetch-Site") &&
        Request.Headers["X-LowCode-Client"] == "Web";

    [HttpGet]
    public async Task<IActionResult> List()
    {
        if (!IsLocalWebRequest) return StatusCode(403);
        return Ok(await _applications.ListAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        if (!IsLocalWebRequest) return StatusCode(403);
        var application = await _applications.GetAsync(id);
        return application == null ? NotFound() : Ok(application);
    }

    [HttpPost]
    [Consumes("application/json")]
    public async Task<IActionResult> Create([FromBody] SaveApplicationRequest request, CancellationToken cancellationToken)
    {
        if (!IsLocalWebRequest) return StatusCode(403);
        try
        {
            var application = await _applications.SaveAsync(null, request, cancellationToken);
            return CreatedAtAction(nameof(Get), new { id = application!.Id }, application);
        }
        catch (ValidationException)
        {
            return BadRequest(new { error = "应用信息无效。" });
        }
        catch (InvalidOperationException)
        {
            return Conflict(new { error = "应用编码已存在，请使用其他编码。" });
        }
    }

    [HttpPut("{id:guid}")]
    [Consumes("application/json")]
    public async Task<IActionResult> Update(Guid id, [FromBody] SaveApplicationRequest request, CancellationToken cancellationToken)
    {
        if (!IsLocalWebRequest) return StatusCode(403);
        try
        {
            var application = await _applications.SaveAsync(id, request, cancellationToken);
            return application == null ? NotFound() : Ok(application);
        }
        catch (ValidationException)
        {
            return BadRequest(new { error = "应用信息无效。" });
        }
        catch (InvalidOperationException)
        {
            return Conflict(new { error = "应用编码已存在，请使用其他编码。" });
        }
    }
}
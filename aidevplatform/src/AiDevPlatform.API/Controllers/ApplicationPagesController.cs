using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using AiDevPlatform.Shared.DTOs.Common;
using AiDevPlatform.Shared.DTOs.ApplicationPage;
using AiDevPlatform.Core.Features.ApplicationPages.Queries;
using AiDevPlatform.Core.Features.ApplicationPages.Commands;

namespace AiDevPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ApplicationPagesController : BaseController
{
    public ApplicationPagesController(IMediator mediator, ILogger<ApplicationPagesController> logger)
        : base(mediator, logger)
    {
    }

    /// <summary>
    /// 获取应用程序页面列表
    /// </summary>
    [HttpGet("application/{applicationId}")]
    public async Task<IActionResult> GetApplicationPages(
        Guid applicationId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = "Name",
        [FromQuery] bool sortDescending = false,
        [FromQuery] bool? isActive = null,
        [FromQuery] bool? isPublic = null,
        [FromQuery] Guid? parentPageId = null)
    {
        try
        {
            var query = new GetApplicationPagesQuery
            {
                ApplicationId = applicationId,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                SortBy = sortBy,
                SortDescending = sortDescending,
                IsActive = isActive,
                IsPublic = isPublic,
                ParentPageId = parentPageId
            };

            var result = await Mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    /// <summary>
    /// 创建应用程序页面
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateApplicationPage([FromBody] CreateApplicationPageCommand command)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await Mediator.Send(command);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetApplicationPage), new { id = result.Data?.Id }, result.Data);
            }

            return BadRequest(result.Message);
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }

    /// <summary>
    /// 获取单个页面
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetApplicationPage(Guid id)
    {
        try
        {
            // This would need a GetApplicationPageByIdQuery
            return Ok("GetApplicationPage endpoint - to be implemented");
        }
        catch (Exception ex)
        {
            return HandleException(ex);
        }
    }
}

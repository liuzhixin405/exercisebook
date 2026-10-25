using MediatR;
using AiDevPlatform.Shared.Common;
using AiDevPlatform.Shared.DTOs.ApplicationPage;

namespace AiDevPlatform.Core.Features.ApplicationPages.Commands;

public class CreateApplicationPageCommand : IRequest<CommandResult<ApplicationPageDto>>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Route { get; set; } = string.Empty;
    public string Type { get; set; } = "Page";
    public string? Layout { get; set; }
    public string? Components { get; set; }
    public string? Styles { get; set; }
    public string? Scripts { get; set; }
    public string? DataBinding { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsPublic { get; set; } = false;
    public int SortOrder { get; set; } = 0;
    public Guid ApplicationId { get; set; }
    public Guid? ParentPageId { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}

using MediatR;
using AiDevPlatform.Shared.DTOs.Common;
using AiDevPlatform.Shared.DTOs.ApplicationPage;

namespace AiDevPlatform.Core.Features.ApplicationPages.Queries;

public class GetApplicationPagesQuery : IRequest<PagedResult<ApplicationPageDto>>
{
    public Guid ApplicationId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public string? SortBy { get; set; } = "Name";
    public bool SortDescending { get; set; } = false;
    public bool? IsActive { get; set; }
    public bool? IsPublic { get; set; }
    public Guid? ParentPageId { get; set; }
}

using MediatR;
using AiDevPlatform.Shared.DTOs.Common;
using AiDevPlatform.Shared.DTOs.Application;

namespace AiDevPlatform.Core.Features.Applications.Queries;

public class GetApplicationsQuery : IRequest<PagedResult<ApplicationDto>>
{
    public PagedRequest Request { get; set; } = new PagedRequest();
    public string UserId { get; set; } = string.Empty;
}

using MediatR;
using AutoMapper;
using Microsoft.Extensions.Logging;
using AiDevPlatform.Core.Interfaces;
using AiDevPlatform.Shared.DTOs.Common;
using AiDevPlatform.Shared.DTOs.Application;

namespace AiDevPlatform.Core.Features.Applications.Queries;

public class GetApplicationsQueryHandler : IRequestHandler<GetApplicationsQuery, PagedResult<ApplicationDto>>
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetApplicationsQueryHandler> _logger;

    public GetApplicationsQueryHandler(IApplicationRepository applicationRepository, IMapper mapper, ILogger<GetApplicationsQueryHandler> logger)
    {
        _applicationRepository = applicationRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResult<ApplicationDto>> Handle(GetApplicationsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _applicationRepository.GetPagedApplicationsAsync(request.Request, request.UserId);
            var applicationDtos = _mapper.Map<IEnumerable<ApplicationDto>>(result.Items);
            return PagedResult<ApplicationDto>.Create(applicationDtos.ToList(), result.TotalCount, result.PageNumber, result.PageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting paged applications for user {UserId}", request.UserId);
            throw;
        }
    }
}

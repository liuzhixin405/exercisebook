using MediatR;
using Microsoft.Extensions.Logging;
using AutoMapper;
using AiDevPlatform.Core.Interfaces;
using AiDevPlatform.Shared.DTOs.Common;
using AiDevPlatform.Shared.DTOs.ApplicationPage;

namespace AiDevPlatform.Core.Features.ApplicationPages.Queries;

public class GetApplicationPagesQueryHandler : IRequestHandler<GetApplicationPagesQuery, PagedResult<ApplicationPageDto>>
{
    private readonly IApplicationPageRepository _applicationPageRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetApplicationPagesQueryHandler> _logger;

    public GetApplicationPagesQueryHandler(
        IApplicationPageRepository applicationPageRepository,
        IMapper mapper,
        ILogger<GetApplicationPagesQueryHandler> logger)
    {
        _applicationPageRepository = applicationPageRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResult<ApplicationPageDto>> Handle(GetApplicationPagesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var pages = await _applicationPageRepository.GetApplicationPagesAsync(request.ApplicationId);
            var pageDtos = _mapper.Map<List<ApplicationPageDto>>(pages);

            // Apply filters
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                pageDtos = pageDtos.Where(p =>
                    p.Name.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (p.Description?.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    p.Route.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            if (request.IsActive.HasValue)
            {
                pageDtos = pageDtos.Where(p => p.IsActive == request.IsActive.Value).ToList();
            }

            if (request.IsPublic.HasValue)
            {
                pageDtos = pageDtos.Where(p => p.IsPublic == request.IsPublic.Value).ToList();
            }

            if (request.ParentPageId.HasValue)
            {
                pageDtos = pageDtos.Where(p => p.ParentPageId == request.ParentPageId.Value).ToList();
            }

            // Apply sorting
            pageDtos = request.SortBy?.ToLower() switch
            {
                "name" => request.SortDescending ? pageDtos.OrderByDescending(p => p.Name).ToList() : pageDtos.OrderBy(p => p.Name).ToList(),
                "route" => request.SortDescending ? pageDtos.OrderByDescending(p => p.Route).ToList() : pageDtos.OrderBy(p => p.Route).ToList(),
                "type" => request.SortDescending ? pageDtos.OrderByDescending(p => p.Type).ToList() : pageDtos.OrderBy(p => p.Type).ToList(),
                "sortorder" => request.SortDescending ? pageDtos.OrderByDescending(p => p.SortOrder).ToList() : pageDtos.OrderBy(p => p.SortOrder).ToList(),
                "createdat" => request.SortDescending ? pageDtos.OrderByDescending(p => p.CreatedAt).ToList() : pageDtos.OrderBy(p => p.CreatedAt).ToList(),
                _ => pageDtos.OrderBy(p => p.SortOrder).ThenBy(p => p.Name).ToList()
            };

            // Apply pagination
            var totalCount = pageDtos.Count;
            var pagedPages = pageDtos
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return PagedResult<ApplicationPageDto>.Create(pagedPages, request.PageNumber, request.PageSize, totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting application pages for application {ApplicationId}", request.ApplicationId);
            throw;
        }
    }
}

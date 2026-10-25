using MediatR;
using Microsoft.Extensions.Logging;
using AiDevPlatform.Core.Interfaces;
using AiDevPlatform.Shared.DTOs.Common;
using AutoMapper;

namespace AiDevPlatform.Core.Features.DataModels.Queries;

public class GetDataModelsQueryHandler : IRequestHandler<GetDataModelsQuery, PagedResult<DataModelDto>>
{
    private readonly IDataModelRepository _dataModelRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<GetDataModelsQueryHandler> _logger;

    public GetDataModelsQueryHandler(
        IDataModelRepository dataModelRepository,
        IMapper mapper,
        ILogger<GetDataModelsQueryHandler> logger)
    {
        _dataModelRepository = dataModelRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResult<DataModelDto>> Handle(GetDataModelsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _dataModelRepository.GetPagedDataModelsAsync(request.Request, request.ApplicationId);
            var dataModels = _mapper.Map<IEnumerable<DataModelDto>>(result.Items);

            return PagedResult<DataModelDto>.Create(dataModels.ToList(), result.TotalCount, result.PageNumber, result.PageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting data models for application {ApplicationId}", request.ApplicationId);
            throw;
        }
    }
}

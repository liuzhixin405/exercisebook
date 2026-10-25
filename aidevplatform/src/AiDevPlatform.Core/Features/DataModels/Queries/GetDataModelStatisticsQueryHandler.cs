using MediatR;
using Microsoft.Extensions.Logging;
using AiDevPlatform.Core.Interfaces;
using AiDevPlatform.Shared.Models;

namespace AiDevPlatform.Core.Features.DataModels.Queries;

public class GetDataModelStatisticsQueryHandler : IRequestHandler<GetDataModelStatisticsQuery, DataModelStatistics>
{
    private readonly IDataModelRepository _dataModelRepository;
    private readonly ILogger<GetDataModelStatisticsQueryHandler> _logger;

    public GetDataModelStatisticsQueryHandler(
        IDataModelRepository dataModelRepository,
        ILogger<GetDataModelStatisticsQueryHandler> logger)
    {
        _dataModelRepository = dataModelRepository;
        _logger = logger;
    }

    public async Task<DataModelStatistics> Handle(GetDataModelStatisticsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var statistics = new DataModelStatistics();

            // Get all data models
            var allDataModels = await _dataModelRepository.GetAllAsync();

            if (request.ApplicationId.HasValue)
            {
                allDataModels = allDataModels.Where(dm => dm.ApplicationId == request.ApplicationId.Value);
            }

            var dataModels = allDataModels.ToList();

            // Calculate basic statistics
            statistics.TotalDataModels = dataModels.Count;
            statistics.SystemDataModels = dataModels.Count(dm => dm.IsSystem);
            statistics.UserDataModels = dataModels.Count(dm => !dm.IsSystem);
            statistics.ReadOnlyDataModels = dataModels.Count(dm => dm.IsReadOnly);

            // Calculate field statistics
            var allFields = new List<DataModelField>();
            var allRelationships = new List<DataModelRelationship>();

            foreach (var dataModel in dataModels)
            {
                var fields = await _dataModelRepository.GetDataModelFieldsAsync(dataModel.Id);
                var relationships = await _dataModelRepository.GetDataModelRelationshipsAsync(dataModel.Id);

                allFields.AddRange(fields);
                allRelationships.AddRange(relationships);
            }

            statistics.TotalFields = allFields.Count;
            statistics.TotalRelationships = allRelationships.Count;

            // Calculate statistics by data type
            statistics.FieldsByDataType = allFields
                .GroupBy(f => f.DataType)
                .ToDictionary(g => g.Key, g => g.Count());

            // Calculate statistics by relationship type
            statistics.RelationshipsByType = allRelationships
                .GroupBy(r => r.Type.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

            // TODO: Implement usage statistics when available
            statistics.MostUsedDataModels = new List<DataModelUsageDto>();

            return statistics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting data model statistics");
            throw;
        }
    }
}

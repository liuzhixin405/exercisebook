using MediatR;

namespace AiDevPlatform.Core.Features.DataModels.Queries;

public class GetDataModelStatisticsQuery : IRequest<DataModelStatistics>
{
    public Guid? ApplicationId { get; set; }
}

public class DataModelStatistics
{
    public int TotalDataModels { get; set; }
    public int SystemDataModels { get; set; }
    public int UserDataModels { get; set; }
    public int ReadOnlyDataModels { get; set; }
    public int TotalFields { get; set; }
    public int TotalRelationships { get; set; }
    public Dictionary<string, int> FieldsByDataType { get; set; } = new();
    public Dictionary<string, int> RelationshipsByType { get; set; } = new();
    public List<DataModelUsageDto> MostUsedDataModels { get; set; } = new();
}

public class DataModelUsageDto
{
    public Guid DataModelId { get; set; }
    public string DataModelName { get; set; } = string.Empty;
    public int UsageCount { get; set; }
}

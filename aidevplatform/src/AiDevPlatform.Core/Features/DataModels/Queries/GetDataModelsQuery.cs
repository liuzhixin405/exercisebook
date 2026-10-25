using MediatR;
using AiDevPlatform.Shared.DTOs.Common;

namespace AiDevPlatform.Core.Features.DataModels.Queries;

public class GetDataModelsQuery : IRequest<PagedResult<DataModelDto>>
{
    public PagedRequest Request { get; set; } = new();
    public Guid ApplicationId { get; set; }
}

public class DataModelDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string TableName { get; set; } = string.Empty;
    public bool IsSystem { get; set; }
    public bool IsReadOnly { get; set; }
    public string? Schema { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Guid ApplicationId { get; set; }
    public Guid CreatedBy { get; set; }
}

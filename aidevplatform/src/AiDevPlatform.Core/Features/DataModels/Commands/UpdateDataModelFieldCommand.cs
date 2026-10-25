using MediatR;
using AiDevPlatform.Shared.Common;

namespace AiDevPlatform.Core.Features.DataModels.Commands;

public class UpdateDataModelFieldCommand : IRequest<CommandResult>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ColumnName { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public int? MaxLength { get; set; }
    public bool IsRequired { get; set; } = false;
    public bool IsPrimaryKey { get; set; } = false;
    public bool IsUnique { get; set; } = false;
    public string? DefaultValue { get; set; }
    public Guid UpdatedBy { get; set; }
}

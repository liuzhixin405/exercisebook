using MediatR;
using AiDevPlatform.Shared.Common;

namespace AiDevPlatform.Core.Features.DataModels.Commands;

public class UpdateDataModelCommand : IRequest<CommandResult>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string TableName { get; set; } = string.Empty;
    public bool IsSystem { get; set; } = false;
    public bool IsReadOnly { get; set; } = false;
    public string? Schema { get; set; }
    public Guid UpdatedBy { get; set; }
}

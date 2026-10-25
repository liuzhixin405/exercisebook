using MediatR;
using AiDevPlatform.Shared.Common;

namespace AiDevPlatform.Core.Features.DataModels.Commands;

public class SyncDataModelFromDatabaseCommand : IRequest<CommandResult>
{
    public Guid DataModelId { get; set; }
    public string ConnectionString { get; set; } = string.Empty;
    public bool OverwriteExisting { get; set; } = false;
    public Guid UpdatedBy { get; set; }
}

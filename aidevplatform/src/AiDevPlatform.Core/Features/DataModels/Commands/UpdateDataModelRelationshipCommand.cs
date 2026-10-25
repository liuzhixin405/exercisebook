using MediatR;
using AiDevPlatform.Shared.Common;

namespace AiDevPlatform.Core.Features.DataModels.Commands;

public class UpdateDataModelRelationshipCommand : IRequest<CommandResult>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ForeignKey { get; set; } = string.Empty;
    public string ReferencedKey { get; set; } = string.Empty;
    public RelationshipType Type { get; set; } = RelationshipType.OneToMany;
    public bool IsRequired { get; set; } = false;
    public bool IsCascadeDelete { get; set; } = false;
    public bool IsCascadeUpdate { get; set; } = false;
    public string? Constraints { get; set; }
    public Guid UpdatedBy { get; set; }
}

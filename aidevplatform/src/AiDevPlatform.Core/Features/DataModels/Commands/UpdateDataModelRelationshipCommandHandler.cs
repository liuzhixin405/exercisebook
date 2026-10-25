using MediatR;
using Microsoft.Extensions.Logging;
using AiDevPlatform.Core.Interfaces;
using AiDevPlatform.Shared.Common;

namespace AiDevPlatform.Core.Features.DataModels.Commands;

public class UpdateDataModelRelationshipCommandHandler : IRequestHandler<UpdateDataModelRelationshipCommand, CommandResult>
{
    private readonly IDataModelRepository _dataModelRepository;
    private readonly ILogger<UpdateDataModelRelationshipCommandHandler> _logger;

    public UpdateDataModelRelationshipCommandHandler(
        IDataModelRepository dataModelRepository,
        ILogger<UpdateDataModelRelationshipCommandHandler> logger)
    {
        _dataModelRepository = dataModelRepository;
        _logger = logger;
    }

    public async Task<CommandResult> Handle(UpdateDataModelRelationshipCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var relationship = await _dataModelRepository.GetRelationshipByIdAsync(request.Id);
            if (relationship == null)
            {
                return CommandResult.FailureResult("关系不存在");
            }

            relationship.Name = request.Name;
            relationship.Description = request.Description;
            relationship.ForeignKey = request.ForeignKey;
            relationship.ReferencedKey = request.ReferencedKey;
            relationship.Type = (AiDevPlatform.Shared.Models.RelationshipType)request.Type;
            relationship.IsRequired = request.IsRequired;
            relationship.IsCascadeDelete = request.IsCascadeDelete;
            relationship.IsCascadeUpdate = request.IsCascadeUpdate;
            relationship.Constraints = request.Constraints;
            relationship.UpdatedBy = request.UpdatedBy.ToString();
            relationship.UpdatedAt = DateTime.UtcNow;

            await _dataModelRepository.UpdateRelationshipAsync(relationship);

            return CommandResult.SuccessResult("关系更新成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating data model relationship {RelationshipId}", request.Id);
            return CommandResult.FailureResult("关系更新失败");
        }
    }
}

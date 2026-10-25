using MediatR;
using Microsoft.Extensions.Logging;
using AiDevPlatform.Core.Interfaces;
using AiDevPlatform.Shared.Common;
using AiDevPlatform.Shared.Models;

namespace AiDevPlatform.Core.Features.DataModels.Commands;

public class AddDataModelRelationshipCommandHandler : IRequestHandler<AddDataModelRelationshipCommand, CommandResult>
{
    private readonly IDataModelRepository _dataModelRepository;
    private readonly ILogger<AddDataModelRelationshipCommandHandler> _logger;

    public AddDataModelRelationshipCommandHandler(
        IDataModelRepository dataModelRepository,
        ILogger<AddDataModelRelationshipCommandHandler> logger)
    {
        _dataModelRepository = dataModelRepository;
        _logger = logger;
    }

    public async Task<CommandResult> Handle(AddDataModelRelationshipCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var dataModel = await _dataModelRepository.GetByIdAsync(request.DataModelId);
            if (dataModel == null)
            {
                return CommandResult.FailureResult("数据模型不存在");
            }

            var referencedDataModel = await _dataModelRepository.GetByIdAsync(request.ReferencedDataModelId);
            if (referencedDataModel == null)
            {
                return CommandResult.FailureResult("引用的数据模型不存在");
            }

            var relationship = new DataModelRelationship
            {
                DataModelId = request.DataModelId,
                Name = request.Name,
                Description = request.Description,
                ForeignKey = request.ForeignKey,
                ReferencedKey = request.ReferencedKey,
                Type = (AiDevPlatform.Shared.Models.RelationshipType)request.Type,
                IsRequired = request.IsRequired,
                IsCascadeDelete = request.IsCascadeDelete,
                IsCascadeUpdate = request.IsCascadeUpdate,
                Constraints = request.Constraints,
                ReferencedDataModelId = request.ReferencedDataModelId,
                CreatedBy = request.CreatedBy.ToString(),
                CreatedAt = DateTime.UtcNow
            };

            await _dataModelRepository.AddRelationshipAsync(relationship);

            return CommandResult.SuccessResult("关系添加成功", relationship.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding relationship to data model {DataModelId}", request.DataModelId);
            return CommandResult.FailureResult("关系添加失败");
        }
    }
}

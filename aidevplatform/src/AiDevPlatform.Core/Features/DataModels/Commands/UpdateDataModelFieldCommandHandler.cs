using MediatR;
using Microsoft.Extensions.Logging;
using AiDevPlatform.Core.Interfaces;
using AiDevPlatform.Shared.Common;

namespace AiDevPlatform.Core.Features.DataModels.Commands;

public class UpdateDataModelFieldCommandHandler : IRequestHandler<UpdateDataModelFieldCommand, CommandResult>
{
    private readonly IDataModelRepository _dataModelRepository;
    private readonly ILogger<UpdateDataModelFieldCommandHandler> _logger;

    public UpdateDataModelFieldCommandHandler(
        IDataModelRepository dataModelRepository,
        ILogger<UpdateDataModelFieldCommandHandler> logger)
    {
        _dataModelRepository = dataModelRepository;
        _logger = logger;
    }

    public async Task<CommandResult> Handle(UpdateDataModelFieldCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var field = await _dataModelRepository.GetFieldByIdAsync(request.Id);
            if (field == null)
            {
                return CommandResult.FailureResult("字段不存在");
            }

            // Check if column name already exists (excluding current field)
            if (await _dataModelRepository.ColumnNameExistsAsync(request.ColumnName, field.DataModelId, request.Id))
            {
                return CommandResult.FailureResult("列名已存在");
            }

            field.Name = request.Name;
            field.Description = request.Description;
            field.ColumnName = request.ColumnName;
            field.DataType = request.DataType;
            field.MaxLength = request.MaxLength;
            field.IsRequired = request.IsRequired;
            field.IsPrimaryKey = request.IsPrimaryKey;
            field.IsUnique = request.IsUnique;
            field.DefaultValue = request.DefaultValue;
            field.UpdatedBy = request.UpdatedBy.ToString();
            field.UpdatedAt = DateTime.UtcNow;

            await _dataModelRepository.UpdateFieldAsync(field);

            return CommandResult.SuccessResult("字段更新成功");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating data model field {FieldId}", request.Id);
            return CommandResult.FailureResult("字段更新失败");
        }
    }
}

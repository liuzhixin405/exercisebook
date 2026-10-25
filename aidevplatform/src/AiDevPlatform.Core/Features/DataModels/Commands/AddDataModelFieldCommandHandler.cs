using MediatR;
using Microsoft.Extensions.Logging;
using AiDevPlatform.Core.Interfaces;
using AiDevPlatform.Shared.Common;
using AiDevPlatform.Shared.Models;

namespace AiDevPlatform.Core.Features.DataModels.Commands;

public class AddDataModelFieldCommandHandler : IRequestHandler<AddDataModelFieldCommand, CommandResult>
{
    private readonly IDataModelRepository _dataModelRepository;
    private readonly ILogger<AddDataModelFieldCommandHandler> _logger;

    public AddDataModelFieldCommandHandler(
        IDataModelRepository dataModelRepository,
        ILogger<AddDataModelFieldCommandHandler> logger)
    {
        _dataModelRepository = dataModelRepository;
        _logger = logger;
    }

    public async Task<CommandResult> Handle(AddDataModelFieldCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var dataModel = await _dataModelRepository.GetByIdAsync(request.DataModelId);
            if (dataModel == null)
            {
                return CommandResult.FailureResult("数据模型不存在");
            }

            // Check if column name already exists
            if (await _dataModelRepository.ColumnNameExistsAsync(request.ColumnName, request.DataModelId))
            {
                return CommandResult.FailureResult("列名已存在");
            }

            var field = new DataModelField
            {
                DataModelId = request.DataModelId,
                Name = request.Name,
                Description = request.Description,
                ColumnName = request.ColumnName,
                DataType = request.DataType,
                MaxLength = request.MaxLength,
                IsRequired = request.IsRequired,
                IsPrimaryKey = request.IsPrimaryKey,
                IsUnique = request.IsUnique,
                DefaultValue = request.DefaultValue,
                CreatedBy = request.CreatedBy.ToString(),
                CreatedAt = DateTime.UtcNow
            };

            await _dataModelRepository.AddFieldAsync(field);

            return CommandResult.SuccessResult("字段添加成功", field.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding field to data model {DataModelId}", request.DataModelId);
            return CommandResult.FailureResult("字段添加失败");
        }
    }
}

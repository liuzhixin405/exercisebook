using MediatR;
using Microsoft.Extensions.Logging;
using AiDevPlatform.Core.Interfaces;

namespace AiDevPlatform.Core.Features.DataModels.Commands;

public class ValidateDataModelCommandHandler : IRequestHandler<ValidateDataModelCommand, DataModelValidationResult>
{
    private readonly IDataModelRepository _dataModelRepository;
    private readonly ILogger<ValidateDataModelCommandHandler> _logger;

    public ValidateDataModelCommandHandler(
        IDataModelRepository dataModelRepository,
        ILogger<ValidateDataModelCommandHandler> logger)
    {
        _dataModelRepository = dataModelRepository;
        _logger = logger;
    }

    public async Task<DataModelValidationResult> Handle(ValidateDataModelCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var result = new DataModelValidationResult { IsValid = true };

            var dataModel = await _dataModelRepository.GetByIdAsync(request.DataModelId);
            if (dataModel == null)
            {
                result.IsValid = false;
                result.Errors.Add("数据模型不存在");
                return result;
            }

            // Validate table name
            if (string.IsNullOrWhiteSpace(dataModel.TableName))
            {
                result.IsValid = false;
                result.Errors.Add("表名不能为空");
            }

            // Validate fields
            var fields = await _dataModelRepository.GetDataModelFieldsAsync(request.DataModelId);
            if (!fields.Any())
            {
                result.Warnings.Add("数据模型没有字段");
            }

            // Check for primary key
            var hasPrimaryKey = fields.Any(f => f.IsPrimaryKey);
            if (!hasPrimaryKey)
            {
                result.Warnings.Add("建议添加主键字段");
            }

            // Validate relationships
            var relationships = await _dataModelRepository.GetDataModelRelationshipsAsync(request.DataModelId);
            foreach (var relationship in relationships)
            {
                if (string.IsNullOrWhiteSpace(relationship.ForeignKey))
                {
                    result.Errors.Add($"关系 '{relationship.Name}' 的外键不能为空");
                    result.IsValid = false;
                }

                if (string.IsNullOrWhiteSpace(relationship.ReferencedKey))
                {
                    result.Errors.Add($"关系 '{relationship.Name}' 的引用键不能为空");
                    result.IsValid = false;
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating data model {DataModelId}", request.DataModelId);
            return new DataModelValidationResult
            {
                IsValid = false,
                Errors = { "验证过程中发生错误" }
            };
        }
    }
}

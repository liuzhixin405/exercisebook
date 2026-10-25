using MediatR;

namespace AiDevPlatform.Core.Features.DataModels.Commands;

public class ValidateDataModelCommand : IRequest<DataModelValidationResult>
{
    public Guid DataModelId { get; set; }
}

public class DataModelValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public List<string> Suggestions { get; set; } = new();
}

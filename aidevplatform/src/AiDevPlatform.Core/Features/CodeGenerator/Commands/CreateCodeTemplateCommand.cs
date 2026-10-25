using MediatR;
using AiDevPlatform.Shared.Common;
using AiDevPlatform.Shared.DTOs.CodeGenerator;

namespace AiDevPlatform.Core.Features.CodeGenerator.Commands;

public class CreateCodeTemplateCommand : IRequest<CommandResult<CodeTemplateDto>>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string TemplateType { get; set; } = string.Empty; // Controller, Service, DTO, etc.
    public string TemplateContent { get; set; } = string.Empty;
    public string? Parameters { get; set; } // JSON parameters definition
    public string? Variables { get; set; } // JSON variables definition
    public bool IsSystem { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public Guid? ApplicationId { get; set; }
    public Guid CreatedBy { get; set; }
}

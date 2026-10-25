namespace AiDevPlatform.Shared.DTOs.CodeGenerator;

public class CodeTemplateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string TemplateType { get; set; } = string.Empty;
    public string TemplateContent { get; set; } = string.Empty;
    public string? Parameters { get; set; }
    public string? Variables { get; set; }
    public bool IsSystem { get; set; }
    public bool IsActive { get; set; }
    public Guid? ApplicationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}

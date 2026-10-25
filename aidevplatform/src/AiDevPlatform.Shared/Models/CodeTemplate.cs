using System.ComponentModel.DataAnnotations;

namespace AiDevPlatform.Shared.Models;

public class CodeTemplate : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(100)]
    public string TemplateType { get; set; } = string.Empty;

    [Required]
    public string TemplateContent { get; set; } = string.Empty;

    public string? Parameters { get; set; } // JSON parameters definition

    public string? Variables { get; set; } // JSON variables definition

    public bool IsSystem { get; set; } = false;

    public bool IsActive { get; set; } = true;

    public Guid? ApplicationId { get; set; }

    // Navigation properties
    public virtual Application? Application { get; set; }
}

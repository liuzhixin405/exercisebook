using System.ComponentModel.DataAnnotations;

namespace AiDevPlatform.Shared.Models;

public class FormField : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? Label { get; set; }

    [Required]
    [MaxLength(50)]
    public string FieldType { get; set; } = string.Empty; // Text, Number, Email, Date, etc.

    public bool IsRequired { get; set; } = false;

    public string? DefaultValue { get; set; }

    public string? ValidationRules { get; set; } // JSON validation rules

    public string? Options { get; set; } // JSON options for select, radio, etc.

    public int SortOrder { get; set; } = 0;

    public bool IsVisible { get; set; } = true;

    [MaxLength(500)]
    public string? Placeholder { get; set; }

    [MaxLength(1000)]
    public string? HelpText { get; set; }

    public Guid FormId { get; set; }

    // Navigation properties
    public virtual Form Form { get; set; } = null!;
}

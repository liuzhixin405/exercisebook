namespace AiDevPlatform.Shared.DTOs.Form;

public class FormFieldDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Label { get; set; }
    public string FieldType { get; set; } = string.Empty; // Text, Number, Email, Date, etc.
    public bool IsRequired { get; set; }
    public string? DefaultValue { get; set; }
    public string? ValidationRules { get; set; } // JSON validation rules
    public string? Options { get; set; } // JSON options for select, radio, etc.
    public int SortOrder { get; set; }
    public bool IsVisible { get; set; } = true;
    public string? Placeholder { get; set; }
    public string? HelpText { get; set; }
    public Guid FormId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}

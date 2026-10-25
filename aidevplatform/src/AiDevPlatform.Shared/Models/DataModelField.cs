using System.ComponentModel.DataAnnotations;

namespace AiDevPlatform.Shared.Models;

public class DataModelField : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(100)]
    public string ColumnName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string DataType { get; set; } = string.Empty;

    public int? MaxLength { get; set; }

    public int? Precision { get; set; }

    public int? Scale { get; set; }

    public bool IsRequired { get; set; } = false;

    public bool IsPrimaryKey { get; set; } = false;

    public bool IsUnique { get; set; } = false;

    public bool IsIndexed { get; set; } = false;

    public string? DefaultValue { get; set; }

    public string? ValidationRules { get; set; } // JSON validation rules

    public string? DisplayFormat { get; set; }

    public int SortOrder { get; set; } = 0;

    public bool IsVisible { get; set; } = true;

    public bool IsReadOnly { get; set; } = false;

    public Guid DataModelId { get; set; }

    // Navigation properties
    public virtual DataModel DataModel { get; set; } = null!;
}

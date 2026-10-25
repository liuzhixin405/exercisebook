using System.ComponentModel.DataAnnotations;

namespace AiDevPlatform.Shared.Models;

public class Form : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(100)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Version { get; set; } = "1.0.0";

    public FormStatus Status { get; set; } = FormStatus.Draft;

    public string? Layout { get; set; } // JSON form layout

    public string? Fields { get; set; } // JSON field definitions

    public string? ValidationRules { get; set; } // JSON validation rules

    public string? BusinessRules { get; set; } // JSON business rules

    public string? Styles { get; set; } // JSON form styles

    public string? Scripts { get; set; } // JSON form scripts

    public string? Permissions { get; set; } // JSON permission rules

    public bool IsActive { get; set; } = true;

    public bool IsSystem { get; set; } = false;

    public Guid ApplicationId { get; set; }

    public Guid? DataModelId { get; set; }

    public Guid? CategoryId { get; set; }

    // Navigation properties
    public virtual Application Application { get; set; } = null!;

    public virtual DataModel? DataModel { get; set; }

    public virtual FormCategory? Category { get; set; }

    public virtual ICollection<FormSubmission> Submissions { get; set; } = new List<FormSubmission>();

    public virtual ICollection<FormPermission> FormPermissions { get; set; } = new List<FormPermission>();
}

public enum FormStatus
{
    Draft,
    Published,
    Archived,
    Deleted
}

using System.ComponentModel.DataAnnotations;

namespace AiDevPlatform.Shared.Models;

public class DataModel : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(100)]
    public string TableName { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Schema { get; set; }

    public string? Fields { get; set; } // JSON field definitions

    public string? Indexes { get; set; } // JSON index definitions

    public string? Relationships { get; set; } // JSON relationship definitions

    public string? ValidationRules { get; set; } // JSON validation rules

    public string? BusinessRules { get; set; } // JSON business rules

    public bool IsSystem { get; set; } = false;

    public bool IsReadOnly { get; set; } = false;

    public Guid ApplicationId { get; set; }

    // Navigation properties
    public virtual Application Application { get; set; } = null!;

    public virtual ICollection<DataModelField> FieldsList { get; set; } = new List<DataModelField>();

    public virtual ICollection<DataModelRelationship> RelationshipsList { get; set; } = new List<DataModelRelationship>();

    public virtual ICollection<Form> Forms { get; set; } = new List<Form>();

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
}

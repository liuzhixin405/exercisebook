using System.ComponentModel.DataAnnotations;

namespace AiDevPlatform.Shared.Models;

public class DataModelRelationship : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(100)]
    public string ForeignKey { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string ReferencedKey { get; set; } = string.Empty;

    // Alias properties for compatibility
    public string ReferencedTableName => ReferencedDataModel?.TableName ?? string.Empty;
    public string ReferencedColumnName => ReferencedKey;
    public string ReferencingColumnName => ForeignKey;

    public RelationshipType Type { get; set; } = RelationshipType.OneToMany;

    public bool IsRequired { get; set; } = false;

    public bool IsCascadeDelete { get; set; } = false;

    public bool IsCascadeUpdate { get; set; } = false;

    public string? Constraints { get; set; } // JSON constraint definitions

    public Guid DataModelId { get; set; }

    public Guid ReferencedDataModelId { get; set; }

    // Navigation properties
    public virtual DataModel DataModel { get; set; } = null!;

    public virtual DataModel ReferencedDataModel { get; set; } = null!;
}

public enum RelationshipType
{
    OneToOne,
    OneToMany,
    ManyToOne,
    ManyToMany
}

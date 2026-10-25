using System.ComponentModel.DataAnnotations;

namespace AiDevPlatform.Shared.DTOs.DataModel;

public class DataModelDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string TableName { get; set; } = string.Empty;

    public string? Schema { get; set; }

    public string? Fields { get; set; }

    public string? Indexes { get; set; }

    public string? Relationships { get; set; }

    public string? ValidationRules { get; set; }

    public string? BusinessRules { get; set; }

    public bool IsSystem { get; set; }

    public bool IsReadOnly { get; set; }

    public Guid ApplicationId { get; set; }

    public string? ApplicationName { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public List<DataModelFieldDto> FieldsList { get; set; } = new();

    public List<DataModelRelationshipDto> RelationshipsList { get; set; } = new();
}

public class DataModelFieldDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string ColumnName { get; set; } = string.Empty;

    public string DataType { get; set; } = string.Empty;

    public int? MaxLength { get; set; }

    public int? Precision { get; set; }

    public int? Scale { get; set; }

    public bool IsRequired { get; set; }

    public bool IsPrimaryKey { get; set; }

    public bool IsUnique { get; set; }

    public bool IsIndexed { get; set; }

    public string? DefaultValue { get; set; }

    public string? ValidationRules { get; set; }

    public string? DisplayFormat { get; set; }

    public int SortOrder { get; set; }

    public bool IsVisible { get; set; }

    public bool IsReadOnly { get; set; }

    public Guid DataModelId { get; set; }
}

public class DataModelRelationshipDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string ForeignKey { get; set; } = string.Empty;

    public string ReferencedKey { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public bool IsRequired { get; set; }

    public bool IsCascadeDelete { get; set; }

    public bool IsCascadeUpdate { get; set; }

    public string? Constraints { get; set; }

    public Guid DataModelId { get; set; }

    public Guid ReferencedDataModelId { get; set; }

    public string? ReferencedDataModelName { get; set; }
}

public class CreateDataModelDto
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

    public string? Fields { get; set; }

    public string? Indexes { get; set; }

    public string? Relationships { get; set; }

    public string? ValidationRules { get; set; }

    public string? BusinessRules { get; set; }

    public bool IsReadOnly { get; set; } = false;

    public Guid ApplicationId { get; set; }
}

public class UpdateDataModelDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string? Schema { get; set; }

    public string? Fields { get; set; }

    public string? Indexes { get; set; }

    public string? Relationships { get; set; }

    public string? ValidationRules { get; set; }

    public string? BusinessRules { get; set; }

    public bool IsReadOnly { get; set; } = false;
}

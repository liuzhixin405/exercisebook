using System.ComponentModel.DataAnnotations;

namespace AiDevPlatform.Shared.DTOs.Component;

public class ComponentDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Version { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string? Properties { get; set; }

    public string? Events { get; set; }

    public string? Styles { get; set; }

    public string? Scripts { get; set; }

    public string? Dependencies { get; set; }

    public string? Icon { get; set; }

    public string? Preview { get; set; }

    public bool IsSystem { get; set; }

    public bool IsActive { get; set; }

    public bool IsPublic { get; set; }

    public Guid? ApplicationId { get; set; }

    public string? ApplicationName { get; set; }

    public Guid? CreatedById { get; set; }

    public string? CreatedByName { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int UsageCount { get; set; }
}

public class CreateComponentDto
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

    public string Type { get; set; } = "UI";

    public string Category { get; set; } = "Basic";

    public string? Properties { get; set; }

    public string? Events { get; set; }

    public string? Styles { get; set; }

    public string? Scripts { get; set; }

    public string? Dependencies { get; set; }

    [MaxLength(500)]
    public string? Icon { get; set; }

    public string? Preview { get; set; }

    public bool IsPublic { get; set; } = false;

    public Guid? ApplicationId { get; set; }
}

public class UpdateComponentDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [MaxLength(20)]
    public string Version { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string? Properties { get; set; }

    public string? Events { get; set; }

    public string? Styles { get; set; }

    public string? Scripts { get; set; }

    public string? Dependencies { get; set; }

    [MaxLength(500)]
    public string? Icon { get; set; }

    public string? Preview { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsPublic { get; set; } = false;
}

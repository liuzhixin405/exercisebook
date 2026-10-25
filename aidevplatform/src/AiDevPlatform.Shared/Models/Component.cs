using System.ComponentModel.DataAnnotations;

namespace AiDevPlatform.Shared.Models;

public class Component : BaseEntity
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

    public ComponentType Type { get; set; } = ComponentType.UI;

    public ComponentCategory Category { get; set; } = ComponentCategory.Basic;

    public string? Properties { get; set; } // JSON property definitions

    public string? Events { get; set; } // JSON event definitions

    public string? Styles { get; set; } // JSON style definitions

    public string? Scripts { get; set; } // JSON script definitions

    public string? Dependencies { get; set; } // JSON dependency definitions

    public string? Icon { get; set; }

    public string? Preview { get; set; } // Base64 preview image

    public bool IsSystem { get; set; } = false;

    public bool IsActive { get; set; } = true;

    public bool IsPublic { get; set; } = false;

    public Guid? ApplicationId { get; set; }

    public Guid? CreatedById { get; set; }

    // Navigation properties
    public virtual Application? Application { get; set; }

    public new virtual User? CreatedBy { get; set; }

    public virtual ICollection<ComponentUsage> Usages { get; set; } = new List<ComponentUsage>();
}

public enum ComponentType
{
    UI,
    Data,
    Business,
    Integration,
    Custom
}

public enum ComponentCategory
{
    Basic,
    Layout,
    Form,
    Data,
    Navigation,
    Media,
    Chart,
    Map,
    Calendar,
    File,
    Communication,
    Social,
    ECommerce,
    Analytics,
    Security,
    Utility
}

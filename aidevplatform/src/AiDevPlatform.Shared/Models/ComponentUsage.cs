using System.ComponentModel.DataAnnotations;

namespace AiDevPlatform.Shared.Models;

public class ComponentUsage : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string UsageId { get; set; } = string.Empty;

    public string? Properties { get; set; } // JSON component properties

    public string? Styles { get; set; } // JSON component styles

    public string? Events { get; set; } // JSON component events

    public string? DataBinding { get; set; } // JSON data binding configuration

    public int SortOrder { get; set; } = 0;

    public bool IsVisible { get; set; } = true;

    public Guid ComponentId { get; set; }

    public Guid? ApplicationId { get; set; }

    public Guid? PageId { get; set; }

    public Guid? ParentUsageId { get; set; }

    // Alias property for compatibility
    public Guid? ApplicationPageId
    {
        get => PageId;
        set => PageId = value;
    }

    // Navigation properties
    public virtual Component Component { get; set; } = null!;

    public virtual Application? Application { get; set; }

    public virtual ApplicationPage? Page { get; set; }

    // Alias property for compatibility
    public virtual ApplicationPage? ApplicationPage
    {
        get => Page;
        set => Page = value;
    }

    public virtual ComponentUsage? ParentUsage { get; set; }

    public virtual ICollection<ComponentUsage> ChildUsages { get; set; } = new List<ComponentUsage>();
}

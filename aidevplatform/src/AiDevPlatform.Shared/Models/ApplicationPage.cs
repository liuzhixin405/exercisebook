using System.ComponentModel.DataAnnotations;

namespace AiDevPlatform.Shared.Models;

public class ApplicationPage : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Code { get; set; }

    [MaxLength(200)]
    public string? Title { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(100)]
    public string Route { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Type { get; set; } = "Page"; // Page, Modal, Component

    public string? Layout { get; set; } // JSON layout configuration

    public string? Components { get; set; } // JSON components configuration

    public string? Styles { get; set; } // JSON styles configuration

    public string? Scripts { get; set; } // JSON scripts configuration

    public string? DataBinding { get; set; } // JSON data binding configuration

    public bool IsActive { get; set; } = true;

    public bool IsPublic { get; set; } = false;

    public int SortOrder { get; set; } = 0;

    public Guid ApplicationId { get; set; }

    public Guid? ParentPageId { get; set; }

    // Navigation properties
    public virtual Application Application { get; set; } = null!;

    public virtual ApplicationPage? ParentPage { get; set; }

    public virtual ICollection<ApplicationPage> ChildPages { get; set; } = new List<ApplicationPage>();

    public virtual ICollection<ComponentUsage> ComponentUsages { get; set; } = new List<ComponentUsage>();
}
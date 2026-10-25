using System.ComponentModel.DataAnnotations;

namespace AiDevPlatform.Shared.Models;

public class Application : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Version { get; set; } = "1.0.0";

    public ApplicationStatus Status { get; set; } = ApplicationStatus.Draft;

    public ApplicationType Type { get; set; } = ApplicationType.Web;

    [MaxLength(500)]
    public string? Icon { get; set; }

    [MaxLength(100)]
    public string? Category { get; set; }

    public string? Configuration { get; set; } // JSON configuration

    public string? Metadata { get; set; } // JSON metadata

    public Guid OwnerId { get; set; }

    public Guid? OrganizationId { get; set; }

    // Navigation properties
    public virtual User Owner { get; set; } = null!;

    public virtual Organization? Organization { get; set; }

    public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; } = new List<ApplicationUser>();

    public virtual ICollection<ApplicationPage> Pages { get; set; } = new List<ApplicationPage>();

    public virtual ICollection<DataModel> DataModels { get; set; } = new List<DataModel>();

    public virtual ICollection<Workflow> Workflows { get; set; } = new List<Workflow>();

    public virtual ICollection<Form> Forms { get; set; } = new List<Form>();

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
}

public enum ApplicationStatus
{
    Draft,
    Published,
    Archived,
    Deleted
}

public enum ApplicationType
{
    Web,
    Mobile,
    Desktop,
    API
}

using System.ComponentModel.DataAnnotations;

namespace AiDevPlatform.Shared.Models;

public class Organization : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Logo { get; set; }

    [MaxLength(200)]
    public string? Website { get; set; }

    [MaxLength(100)]
    public string? ContactEmail { get; set; }

    [MaxLength(50)]
    public string? ContactPhone { get; set; }

    // Alias properties for compatibility
    public string? Email => ContactEmail;
    public string? Phone => ContactPhone;
    public bool IsActive => Status == OrganizationStatus.Active;

    [MaxLength(500)]
    public string? Address { get; set; }

    public OrganizationStatus Status { get; set; } = OrganizationStatus.Active;

    public Guid? ParentId { get; set; }

    // Navigation properties
    public virtual Organization? Parent { get; set; }

    public virtual ICollection<Organization> Children { get; set; } = new List<Organization>();

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual ICollection<OrganizationUser> OrganizationUsers { get; set; } = new List<OrganizationUser>();
}

public enum OrganizationStatus
{
    Active,
    Inactive,
    Suspended
}

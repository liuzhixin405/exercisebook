using System.ComponentModel.DataAnnotations;

namespace AiDevPlatform.Shared.Models;

public class Role : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(100)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Category { get; set; }

    public RoleType Type { get; set; } = RoleType.Custom;

    public bool IsSystem { get; set; } = false;

    public bool IsActive { get; set; } = true;

    public Guid? ApplicationId { get; set; }

    public Guid? OrganizationId { get; set; }

    // Navigation properties
    public virtual Application? Application { get; set; }

    public virtual Organization? Organization { get; set; }

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

public enum RoleType
{
    System,
    Application,
    Organization,
    Custom
}

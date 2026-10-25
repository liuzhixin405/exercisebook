using System.ComponentModel.DataAnnotations;

namespace AiDevPlatform.Shared.Models;

public class Permission : BaseEntity
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

    public PermissionType Type { get; set; } = PermissionType.Resource;

    public string? Resource { get; set; }

    public string? Actions { get; set; } // JSON allowed actions

    // Alias property for compatibility
    public string? Action => Actions;

    public string? Conditions { get; set; } // JSON permission conditions

    public bool IsSystem { get; set; } = false;

    public bool IsActive { get; set; } = true;

    public Guid? ApplicationId { get; set; }

    // Navigation properties
    public virtual Application? Application { get; set; }

    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();

    public virtual ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();
}

public enum PermissionType
{
    Resource,
    Action,
    Data,
    Feature,
    System
}

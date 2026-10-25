using System.ComponentModel.DataAnnotations;

namespace AiDevPlatform.Shared.Models;

public class OrganizationUser : BaseEntity
{
    public Guid OrganizationId { get; set; }

    public Guid UserId { get; set; }

    public OrganizationUserRole Role { get; set; } = OrganizationUserRole.Member;

    public OrganizationUserStatus Status { get; set; } = OrganizationUserStatus.Active;

    public DateTime? JoinedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastAccessAt { get; set; }

    public string? Permissions { get; set; } // JSON permission definitions

    public string? Settings { get; set; } // JSON user settings

    // Navigation properties
    public virtual Organization Organization { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}

public enum OrganizationUserRole
{
    Owner,
    Admin,
    Manager,
    Member,
    Viewer
}

public enum OrganizationUserStatus
{
    Active,
    Inactive,
    Suspended,
    Pending
}

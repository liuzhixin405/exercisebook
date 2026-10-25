using System.ComponentModel.DataAnnotations;

namespace AiDevPlatform.Shared.Models;

public class ApplicationUser : BaseEntity
{
    public Guid ApplicationId { get; set; }

    public Guid UserId { get; set; }

    public ApplicationUserRole Role { get; set; } = ApplicationUserRole.User;

    public ApplicationUserStatus Status { get; set; } = ApplicationUserStatus.Active;

    public DateTime? JoinedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastAccessAt { get; set; }

    public string? Permissions { get; set; } // JSON permission definitions

    public string? Settings { get; set; } // JSON user settings

    // Navigation properties
    public virtual Application Application { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}

public enum ApplicationUserRole
{
    Owner,
    Admin,
    Developer,
    Designer,
    User,
    Viewer
}

public enum ApplicationUserStatus
{
    Active,
    Inactive,
    Suspended,
    Pending
}

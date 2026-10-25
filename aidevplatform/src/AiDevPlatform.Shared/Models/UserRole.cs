using System.ComponentModel.DataAnnotations;

namespace AiDevPlatform.Shared.Models;

public class UserRole : BaseEntity
{
    public Guid UserId { get; set; }

    public Guid RoleId { get; set; }

    public DateTime? AssignedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ExpiresAt { get; set; }

    public string? AssignedBy { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}

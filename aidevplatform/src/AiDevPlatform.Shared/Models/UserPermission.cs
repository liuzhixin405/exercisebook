using System.ComponentModel.DataAnnotations;

namespace AiDevPlatform.Shared.Models;

public class UserPermission : BaseEntity
{
    public Guid UserId { get; set; }

    public Guid PermissionId { get; set; }

    public bool IsGranted { get; set; } = true;

    public string? Conditions { get; set; } // JSON additional conditions

    public DateTime? ExpiresAt { get; set; }

    public string? AssignedBy { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;

    public virtual Permission Permission { get; set; } = null!;
}

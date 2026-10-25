using System.ComponentModel.DataAnnotations;

namespace AiDevPlatform.Shared.Models;

public class RolePermission : BaseEntity
{
    public Guid RoleId { get; set; }

    public Guid PermissionId { get; set; }

    public bool IsGranted { get; set; } = true;

    public string? Conditions { get; set; } // JSON additional conditions

    public DateTime? ExpiresAt { get; set; }

    // Navigation properties
    public virtual Role Role { get; set; } = null!;

    public virtual Permission Permission { get; set; } = null!;
}

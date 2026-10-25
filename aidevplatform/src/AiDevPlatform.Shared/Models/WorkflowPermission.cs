using System.ComponentModel.DataAnnotations;

namespace AiDevPlatform.Shared.Models;

public class WorkflowPermission : BaseEntity
{
    public Guid WorkflowId { get; set; }

    public Guid? UserId { get; set; }

    public Guid? RoleId { get; set; }

    public string? Permission { get; set; } // View, Execute, Manage, etc.

    // Alias property for compatibility
    public Guid PermissionId
    {
        get => WorkflowId;
        set => WorkflowId = value;
    }

    public bool IsGranted { get; set; } = true;

    public string? Conditions { get; set; } // JSON additional conditions

    public DateTime? ExpiresAt { get; set; }

    // Navigation properties
    public virtual Workflow Workflow { get; set; } = null!;

    public virtual User? User { get; set; }

    public virtual Role? Role { get; set; }
}

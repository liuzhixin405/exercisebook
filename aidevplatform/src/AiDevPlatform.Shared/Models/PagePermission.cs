using System.ComponentModel.DataAnnotations;

namespace AiDevPlatform.Shared.Models;

public class PagePermission : BaseEntity
{
    public Guid PageId { get; set; }

    public Guid? UserId { get; set; }

    public Guid? RoleId { get; set; }

    public string? Permission { get; set; } // View, Edit, Delete, etc.

    // Alias properties for compatibility
    public Guid ApplicationPageId
    {
        get => PageId;
        set => PageId = value;
    }
    public Guid PermissionId
    {
        get => PageId;
        set => PageId = value;
    }

    public bool IsGranted { get; set; } = true;

    public string? Conditions { get; set; } // JSON additional conditions

    public DateTime? ExpiresAt { get; set; }

    // Navigation properties
    public virtual ApplicationPage Page { get; set; } = null!;

    public virtual ApplicationPage ApplicationPage
    {
        get => Page;
        set => Page = value;
    }

    public virtual User? User { get; set; }

    public virtual Role? Role { get; set; }
}

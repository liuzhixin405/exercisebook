using System.ComponentModel.DataAnnotations;

namespace AiDevPlatform.Shared.Models;

public class Workflow : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(100)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Version { get; set; } = "1.0.0";

    public WorkflowStatus Status { get; set; } = WorkflowStatus.Draft;

    public string? Definition { get; set; } // JSON workflow definition

    public string? Variables { get; set; } // JSON workflow variables

    public string? Triggers { get; set; } // JSON trigger definitions

    public string? Actions { get; set; } // JSON action definitions

    public string? Conditions { get; set; } // JSON condition definitions

    public string? Forms { get; set; } // JSON form definitions

    public string? Notifications { get; set; } // JSON notification definitions

    public bool IsActive { get; set; } = true;

    public bool IsSystem { get; set; } = false;

    public Guid ApplicationId { get; set; }

    public Guid? CategoryId { get; set; }

    // Navigation properties
    public virtual Application Application { get; set; } = null!;

    public virtual WorkflowCategory? Category { get; set; }

    public virtual ICollection<WorkflowInstance> Instances { get; set; } = new List<WorkflowInstance>();

    public virtual ICollection<WorkflowPermission> Permissions { get; set; } = new List<WorkflowPermission>();
}

public enum WorkflowStatus
{
    Draft,
    Published,
    Archived,
    Deleted
}

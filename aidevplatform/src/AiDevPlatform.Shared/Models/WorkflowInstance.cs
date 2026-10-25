using System.ComponentModel.DataAnnotations;

namespace AiDevPlatform.Shared.Models;

public class WorkflowInstance : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string InstanceId { get; set; } = string.Empty;

    public WorkflowInstanceStatus Status { get; set; } = WorkflowInstanceStatus.Running;

    public string? CurrentStep { get; set; }

    public string? Data { get; set; } // JSON instance data

    public string? History { get; set; } // JSON execution history

    public string? Variables { get; set; } // JSON instance variables

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime? SuspendedAt { get; set; }

    public string? SuspendedReason { get; set; }

    public string? ErrorMessage { get; set; }

    public int Priority { get; set; } = 0;

    public Guid WorkflowId { get; set; }

    public Guid? StartedBy { get; set; }

    public Guid? AssignedTo { get; set; }

    // Navigation properties
    public virtual Workflow Workflow { get; set; } = null!;

    public virtual User? StartedByUser { get; set; }

    public virtual User? AssignedToUser { get; set; }

    public virtual ICollection<WorkflowTask> Tasks { get; set; } = new List<WorkflowTask>();
}

public enum WorkflowInstanceStatus
{
    Running,
    Completed,
    Suspended,
    Cancelled,
    Error
}

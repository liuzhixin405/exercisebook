using System.ComponentModel.DataAnnotations;

namespace AiDevPlatform.Shared.Models;

public class WorkflowTask : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(100)]
    public string TaskId { get; set; } = string.Empty;

    public WorkflowTaskStatus Status { get; set; } = WorkflowTaskStatus.Pending;

    public WorkflowTaskType Type { get; set; } = WorkflowTaskType.User;

    public string? Data { get; set; } // JSON task data

    public string? Form { get; set; } // JSON form definition

    public string? Actions { get; set; } // JSON available actions

    public DateTime? AssignedAt { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime? DueDate { get; set; }

    public int Priority { get; set; } = 0;

    public string? Comments { get; set; }

    public string? Result { get; set; } // JSON task result

    public Guid WorkflowInstanceId { get; set; }

    public Guid? AssignedTo { get; set; }

    public new Guid? CreatedBy { get; set; }

    // Navigation properties
    public virtual WorkflowInstance WorkflowInstance { get; set; } = null!;

    public virtual User? AssignedToUser { get; set; }

    public virtual User? CreatedByUser { get; set; }
}

public enum WorkflowTaskStatus
{
    Pending,
    InProgress,
    Completed,
    Cancelled,
    Skipped
}

public enum WorkflowTaskType
{
    User,
    System,
    Service,
    Timer,
    Condition
}

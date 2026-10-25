using AiDevPlatform.Shared.Models;

namespace AiDevPlatform.Shared.DTOs.Workflow;

public class WorkflowTaskInfo
{
    public Guid TaskId { get; set; }
    public string TaskName { get; set; } = string.Empty;
    public string TaskType { get; set; } = string.Empty;
    public WorkflowTaskStatus Status { get; set; }
    public WorkflowTaskPriority Priority { get; set; }
    public Guid? AssignedTo { get; set; }
    public string? AssignedToName { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Comments { get; set; }
    public string? Result { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace AiDevPlatform.Shared.DTOs.Workflow;

public class WorkflowDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Version { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string? Definition { get; set; }

    public string? Variables { get; set; }

    public string? Triggers { get; set; }

    public string? Actions { get; set; }

    public string? Conditions { get; set; }

    public string? Forms { get; set; }

    public string? Notifications { get; set; }

    public bool IsActive { get; set; }

    public bool IsSystem { get; set; }

    public Guid ApplicationId { get; set; }

    public string? ApplicationName { get; set; }

    public Guid? CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int InstanceCount { get; set; }

    public int ActiveInstanceCount { get; set; }
}

public class CreateWorkflowDto
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

    public string? Definition { get; set; }

    public string? Variables { get; set; }

    public string? Triggers { get; set; }

    public string? Actions { get; set; }

    public string? Conditions { get; set; }

    public string? Forms { get; set; }

    public string? Notifications { get; set; }

    public bool IsActive { get; set; } = true;

    public Guid ApplicationId { get; set; }

    public Guid? CategoryId { get; set; }
}

public class UpdateWorkflowDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [MaxLength(20)]
    public string Version { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string? Definition { get; set; }

    public string? Variables { get; set; }

    public string? Triggers { get; set; }

    public string? Actions { get; set; }

    public string? Conditions { get; set; }

    public string? Forms { get; set; }

    public string? Notifications { get; set; }

    public bool IsActive { get; set; } = true;

    public Guid? CategoryId { get; set; }
}

public class WorkflowInstanceDto
{
    public Guid Id { get; set; }

    public string InstanceId { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string? CurrentStep { get; set; }

    public string? Data { get; set; }

    public string? History { get; set; }

    public string? Variables { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime? SuspendedAt { get; set; }

    public string? SuspendedReason { get; set; }

    public string? ErrorMessage { get; set; }

    public int Priority { get; set; }

    public Guid WorkflowId { get; set; }

    public string? WorkflowName { get; set; }

    public Guid? StartedBy { get; set; }

    public string? StartedByName { get; set; }

    public Guid? AssignedTo { get; set; }

    public string? AssignedToName { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public List<WorkflowTaskDto> Tasks { get; set; } = new();
}

public class WorkflowTaskDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string TaskId { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string? Data { get; set; }

    public string? Form { get; set; }

    public string? Actions { get; set; }

    public DateTime? AssignedAt { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime? DueDate { get; set; }

    public int Priority { get; set; }

    public string? Comments { get; set; }

    public string? Result { get; set; }

    public Guid WorkflowInstanceId { get; set; }

    public Guid? AssignedTo { get; set; }

    public string? AssignedToName { get; set; }

    public Guid? CreatedBy { get; set; }

    public string? CreatedByName { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}

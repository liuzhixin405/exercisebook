using MediatR;
using AiDevPlatform.Shared.Common;
using AiDevPlatform.Shared.Models;
using AiDevPlatform.Shared.DTOs.Workflow;

namespace AiDevPlatform.Core.Features.Workflows.Commands;

public class StartWorkflowCommand : IRequest<CommandResult<WorkflowInstanceResult>>
{
    public Guid WorkflowId { get; set; }
    public string? Parameters { get; set; } // JSON parameters
    public string? Variables { get; set; } // JSON variables
    public string? Context { get; set; } // JSON context data
    public Guid StartedBy { get; set; }
}

public class WorkflowInstanceResult
{
    public Guid InstanceId { get; set; }
    public string InstanceNumber { get; set; } = string.Empty;
    public WorkflowInstanceStatus Status { get; set; }
    public string? Message { get; set; }
    public DateTime StartedAt { get; set; }
    public List<WorkflowTaskInfo> Tasks { get; set; } = new();
}

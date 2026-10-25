using MediatR;
using AiDevPlatform.Shared.Common;
using AiDevPlatform.Shared.Models;

namespace AiDevPlatform.Core.Features.Workflows.Commands;

public class UpdateTaskPriorityCommand : IRequest<CommandResult>
{
    public Guid TaskId { get; set; }
    public WorkflowTaskPriority Priority { get; set; }
    public Guid UpdatedBy { get; set; }
}

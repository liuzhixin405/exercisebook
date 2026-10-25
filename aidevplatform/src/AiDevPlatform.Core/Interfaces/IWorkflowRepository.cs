using AiDevPlatform.Shared.Models;
using AiDevPlatform.Shared.DTOs.Common;

namespace AiDevPlatform.Core.Interfaces;

public interface IWorkflowRepository : IRepository<Workflow>
{
    Task<PagedResult<Workflow>> GetPagedWorkflowsAsync(PagedRequest request, Guid applicationId);
    Task<IEnumerable<Workflow>> GetApplicationWorkflowsAsync(Guid applicationId);
    Task<IEnumerable<Workflow>> GetActiveWorkflowsAsync();
    Task<Workflow?> GetByCodeAsync(string code);
    Task<bool> CodeExistsAsync(string code, Guid? excludeId = null);
    Task<WorkflowInstance?> GetWorkflowInstanceAsync(string instanceId);
    Task<IEnumerable<WorkflowInstance>> GetUserWorkflowInstancesAsync(Guid userId);
    Task<IEnumerable<WorkflowTask>> GetUserTasksAsync(Guid userId);
    Task<IEnumerable<WorkflowTask>> GetPendingTasksAsync();
    Task UpdateWorkflowInstanceStatusAsync(string instanceId, WorkflowInstanceStatus status);
    Task UpdateTaskStatusAsync(Guid taskId, WorkflowTaskStatus status);
    Task<WorkflowTask?> GetTaskAsync(Guid taskId);
    Task<IEnumerable<WorkflowInstance>> GetWorkflowInstancesAsync(Guid workflowId);
}

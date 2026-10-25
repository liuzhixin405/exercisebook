using AiDevPlatform.Shared.Models;

namespace AiDevPlatform.Core.Interfaces;

public interface IWorkflowEngineService
{
    Task<WorkflowInstance> StartWorkflowAsync(Guid workflowId, Guid userId, string? data = null);
    Task<WorkflowInstance> StartWorkflowAsync(string workflowCode, Guid userId, string? data = null);
    Task CompleteTaskAsync(Guid taskId, Guid userId, string? result = null);
    Task CompleteTaskAsync(string taskId, Guid userId, string? result = null);
    Task SuspendWorkflowAsync(string instanceId, string reason);
    Task ResumeWorkflowAsync(string instanceId);
    Task CancelWorkflowAsync(string instanceId, string reason);
    Task<WorkflowInstance?> GetWorkflowInstanceAsync(string instanceId);
    Task<IEnumerable<WorkflowTask>> GetUserTasksAsync(Guid userId);
    Task<IEnumerable<WorkflowTask>> GetPendingTasksAsync();
    Task<WorkflowTask?> GetTaskAsync(Guid taskId);
    Task<WorkflowTask?> GetTaskAsync(string taskId);
    Task AssignTaskAsync(Guid taskId, Guid userId);
    Task UnassignTaskAsync(Guid taskId);
    Task UpdateTaskPriorityAsync(Guid taskId, int priority);
    Task SetTaskDueDateAsync(Guid taskId, DateTime dueDate);
    Task AddTaskCommentAsync(Guid taskId, string comment, Guid userId);
    Task<bool> ValidateWorkflowDefinitionAsync(string definition);
    Task<WorkflowInstance> ExecuteWorkflowStepAsync(string instanceId, string stepId, string? data = null);
    Task<IEnumerable<WorkflowInstance>> GetWorkflowInstancesAsync(Guid workflowId);
    Task<IEnumerable<WorkflowInstance>> GetUserWorkflowInstancesAsync(Guid userId);
    Task<WorkflowExecutionResult> ExecuteWorkflowAsync(Workflow workflow, string? inputData = null);
}

public class WorkflowExecutionResult
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string? OutputData { get; set; }
    public TimeSpan ExecutionTime { get; set; }
    public List<string> ExecutionSteps { get; set; } = new();
}

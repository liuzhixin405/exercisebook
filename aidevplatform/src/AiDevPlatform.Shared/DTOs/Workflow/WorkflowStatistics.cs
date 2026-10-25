namespace AiDevPlatform.Shared.DTOs.Workflow;

public class WorkflowStatistics
{
    public int TotalWorkflows { get; set; }
    public int ActiveWorkflows { get; set; }
    public int InactiveWorkflows { get; set; }
    public int PublicWorkflows { get; set; }
    public int PrivateWorkflows { get; set; }
    public int TotalInstances { get; set; }
    public int RunningInstances { get; set; }
    public int CompletedInstances { get; set; }
    public int FailedInstances { get; set; }
    public int CancelledInstances { get; set; }
    public int SuspendedInstances { get; set; }
    public double SuccessRate { get; set; }
    public double AverageExecutionTime { get; set; } // in minutes
    public int TotalTasks { get; set; }
    public int PendingTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int OverdueTasks { get; set; }
    public Dictionary<string, int> InstancesByStatus { get; set; } = new();
    public Dictionary<string, int> TasksByStatus { get; set; } = new();
    public List<WorkflowTrend> ExecutionTrends { get; set; } = new();
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}

public class WorkflowTrend
{
    public DateTime Date { get; set; }
    public int Instances { get; set; }
    public int CompletedInstances { get; set; }
    public int FailedInstances { get; set; }
    public double AverageExecutionTime { get; set; }
}

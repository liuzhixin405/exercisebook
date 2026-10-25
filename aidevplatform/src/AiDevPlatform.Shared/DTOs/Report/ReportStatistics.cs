namespace AiDevPlatform.Shared.DTOs.Report;

public class ReportStatistics
{
    public int TotalReports { get; set; }
    public int ActiveReports { get; set; }
    public int InactiveReports { get; set; }
    public int PublicReports { get; set; }
    public int PrivateReports { get; set; }
    public int TotalExecutions { get; set; }
    public int SuccessfulExecutions { get; set; }
    public int FailedExecutions { get; set; }
    public int PendingExecutions { get; set; }
    public double SuccessRate { get; set; }
    public double AverageExecutionTime { get; set; } // in seconds
    public Dictionary<string, int> ReportsByType { get; set; } = new();
    public Dictionary<string, int> ExecutionsByStatus { get; set; } = new();
    public List<ReportTrend> ExecutionTrends { get; set; } = new();
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}

public class ReportTrend
{
    public DateTime Date { get; set; }
    public int Executions { get; set; }
    public int SuccessfulExecutions { get; set; }
    public int FailedExecutions { get; set; }
    public double AverageExecutionTime { get; set; }
}

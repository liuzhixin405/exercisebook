namespace AiDevPlatform.Shared.DTOs.Deployment;

public class DeploymentMetrics
{
    public int TotalDeployments { get; set; }
    public int SuccessfulDeployments { get; set; }
    public int FailedDeployments { get; set; }
    public double SuccessRate { get; set; }
    public double AverageDeploymentTime { get; set; }
    public double MedianDeploymentTime { get; set; }
    public double P95DeploymentTime { get; set; }
    public double P99DeploymentTime { get; set; }
    public int DeploymentsLast24Hours { get; set; }
    public int DeploymentsLast7Days { get; set; }
    public int DeploymentsLast30Days { get; set; }
    public Dictionary<string, int> DeploymentsByHour { get; set; } = new();
    public Dictionary<string, int> DeploymentsByDay { get; set; } = new();
    public Dictionary<string, double> AverageTimeByEnvironment { get; set; } = new();
    public List<DeploymentFailureReason> TopFailureReasons { get; set; } = new();
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}

public class DeploymentFailureReason
{
    public string Reason { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}

namespace AiDevPlatform.Shared.DTOs.Deployment;

public class DeploymentStatistics
{
    public int TotalDeployments { get; set; }
    public int SuccessfulDeployments { get; set; }
    public int FailedDeployments { get; set; }
    public int PendingDeployments { get; set; }
    public int CancelledDeployments { get; set; }
    public double SuccessRate { get; set; }
    public double AverageDeploymentTime { get; set; } // in minutes
    public double FastestDeploymentTime { get; set; }
    public double SlowestDeploymentTime { get; set; }
    public Dictionary<string, int> DeploymentsByEnvironment { get; set; } = new();
    public Dictionary<string, int> DeploymentsByStatus { get; set; } = new();
    public List<DeploymentTrend> DeploymentTrends { get; set; } = new();
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}

public class DeploymentTrend
{
    public DateTime Date { get; set; }
    public int SuccessfulDeployments { get; set; }
    public int FailedDeployments { get; set; }
    public double AverageTime { get; set; }
}

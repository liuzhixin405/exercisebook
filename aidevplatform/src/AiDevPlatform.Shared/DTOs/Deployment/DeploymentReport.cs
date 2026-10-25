namespace AiDevPlatform.Shared.DTOs.Deployment;

public class DeploymentReport
{
    public Guid Id { get; set; }
    public string ReportName { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty; // Summary, Detailed, Trend, etc.
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PeriodStart { get; set; }
    public DateTime? PeriodEnd { get; set; }
    public Guid ApplicationId { get; set; }
    public string? Environment { get; set; }
    public DeploymentStatistics Statistics { get; set; } = new();
    public List<DeploymentHistoryDto> DeploymentHistory { get; set; } = new();
    public List<DeploymentTrend> Trends { get; set; } = new();
    public string? Summary { get; set; }
    public List<string> Recommendations { get; set; } = new();
    public string? GeneratedBy { get; set; }
}

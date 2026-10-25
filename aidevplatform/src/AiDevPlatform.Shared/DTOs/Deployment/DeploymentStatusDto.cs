namespace AiDevPlatform.Shared.DTOs.Deployment;

public class DeploymentStatusDto
{
    public Guid DeploymentId { get; set; }
    public DeploymentStatus Status { get; set; }
    public string? CurrentStep { get; set; }
    public int ProgressPercentage { get; set; }
    public string? Message { get; set; }
    public DateTime LastUpdated { get; set; }
    public List<string> CompletedSteps { get; set; } = new();
    public List<string> PendingSteps { get; set; } = new();
    public List<string> FailedSteps { get; set; } = new();
}

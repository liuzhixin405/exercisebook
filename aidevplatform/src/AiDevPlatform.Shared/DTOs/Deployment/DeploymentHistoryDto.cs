namespace AiDevPlatform.Shared.DTOs.Deployment;

public class DeploymentHistoryDto
{
    public Guid Id { get; set; }
    public string Version { get; set; } = string.Empty;
    public DeploymentStatus Status { get; set; }
    public string? Environment { get; set; }
    public string? TargetUrl { get; set; }
    public DateTime? DeployedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public TimeSpan? Duration { get; set; }
    public Guid ApplicationId { get; set; }
    public Guid? DeployedBy { get; set; }
    public string? DeployedByUserName { get; set; }
    public string? Notes { get; set; }
    public List<DeploymentLogDto> Logs { get; set; } = new();
}

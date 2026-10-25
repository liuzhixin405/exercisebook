namespace AiDevPlatform.Shared.DTOs.Deployment;

public class DeploymentLogDto
{
    public Guid Id { get; set; }
    public string Level { get; set; } = string.Empty; // Info, Warning, Error, Debug
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
    public DateTime Timestamp { get; set; }
    public Guid DeploymentId { get; set; }
    public string? Source { get; set; } // Build, Deploy, Test, etc.
}

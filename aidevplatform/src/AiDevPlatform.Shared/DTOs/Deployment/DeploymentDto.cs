using AiDevPlatform.Shared.DTOs.Application;
using AiDevPlatform.Shared.DTOs.User;

namespace AiDevPlatform.Shared.DTOs.Deployment;

public class DeploymentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Version { get; set; } = string.Empty;
    public DeploymentStatus Status { get; set; }
    public string? Environment { get; set; }
    public string? TargetUrl { get; set; }
    public string? Configuration { get; set; } // JSON configuration
    public string? BuildLog { get; set; }
    public string? DeployLog { get; set; }
    public DateTime? DeployedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public Guid ApplicationId { get; set; }
    public Guid? DeployedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation properties
    public ApplicationDto? Application { get; set; }
    public UserDto? DeployedByUser { get; set; }
}

public enum DeploymentStatus
{
    Pending,
    Building,
    Deploying,
    Success,
    Failed,
    Cancelled,
    RollingBack,
    RolledBack
}

using System.ComponentModel.DataAnnotations;

namespace AiDevPlatform.Shared.Models;

public class Deployment : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(50)]
    public string Version { get; set; } = string.Empty;

    public DeploymentStatus Status { get; set; } = DeploymentStatus.Pending;

    [MaxLength(100)]
    public string? Environment { get; set; }

    [MaxLength(500)]
    public string? TargetUrl { get; set; }

    public string? Configuration { get; set; } // JSON configuration

    public string? BuildLog { get; set; }

    public string? DeployLog { get; set; }

    public DateTime? DeployedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public Guid ApplicationId { get; set; }

    public Guid? DeployedBy { get; set; }

    // Navigation properties
    public virtual Application Application { get; set; } = null!;

    public virtual User? DeployedByUser { get; set; }
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

using AiDevPlatform.Shared.DTOs.Application;

namespace AiDevPlatform.Shared.DTOs.Deployment;

public class DeploymentConfigurationDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ConfigurationType { get; set; } = string.Empty; // Build, Deploy, Test, etc.
    public string Configuration { get; set; } = string.Empty; // JSON configuration
    public bool IsActive { get; set; } = true;
    public bool IsDefault { get; set; } = false;
    public Guid ApplicationId { get; set; }
    public Guid? EnvironmentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation properties
    public ApplicationDto? Application { get; set; }
    public DeploymentEnvironmentDto? Environment { get; set; }
}

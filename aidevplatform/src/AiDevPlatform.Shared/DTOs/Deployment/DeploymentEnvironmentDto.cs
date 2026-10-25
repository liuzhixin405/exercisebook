using AiDevPlatform.Shared.DTOs.Application;

namespace AiDevPlatform.Shared.DTOs.Deployment;

public class DeploymentEnvironmentDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string EnvironmentType { get; set; } = string.Empty; // Development, Staging, Production
    public string? BaseUrl { get; set; }
    public string? Configuration { get; set; } // JSON environment configuration
    public bool IsActive { get; set; } = true;
    public bool IsDefault { get; set; } = false;
    public Guid ApplicationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation properties
    public ApplicationDto? Application { get; set; }
}

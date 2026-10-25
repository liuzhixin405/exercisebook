namespace AiDevPlatform.Shared.DTOs.Application;

public class CreateApplicationDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Logo { get; set; }
    public string? Theme { get; set; }
    public string? Configuration { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsPublic { get; set; } = false;
    public string? Version { get; set; }
    public Guid OrganizationId { get; set; }
}

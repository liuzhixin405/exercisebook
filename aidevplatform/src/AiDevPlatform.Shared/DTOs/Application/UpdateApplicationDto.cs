namespace AiDevPlatform.Shared.DTOs.Application;

public class UpdateApplicationDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? Logo { get; set; }
    public string? Theme { get; set; }
    public string? Configuration { get; set; }
    public bool IsActive { get; set; }
    public bool IsPublic { get; set; }
    public string? Version { get; set; }
}

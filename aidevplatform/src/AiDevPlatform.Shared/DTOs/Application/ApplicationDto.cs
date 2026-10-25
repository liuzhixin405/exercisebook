namespace AiDevPlatform.Shared.DTOs.Application;

public class ApplicationDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Code { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Logo { get; set; }
    public string? Theme { get; set; }
    public string? Configuration { get; set; } // JSON configuration
    public bool IsActive { get; set; } = true;
    public bool IsPublic { get; set; } = false;
    public string? Version { get; set; }
    public DateTime? PublishedAt { get; set; }
    public Guid OrganizationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation properties
    public OrganizationDto? Organization { get; set; }
    public List<ApplicationPageDto> Pages { get; set; } = new();
}
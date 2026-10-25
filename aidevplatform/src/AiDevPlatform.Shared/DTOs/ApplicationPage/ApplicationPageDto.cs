namespace AiDevPlatform.Shared.DTOs.ApplicationPage;

public class ApplicationPageDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Route { get; set; } = string.Empty;
    public string Type { get; set; } = "Page";
    public string? Layout { get; set; }
    public string? Components { get; set; }
    public string? Styles { get; set; }
    public string? Scripts { get; set; }
    public string? DataBinding { get; set; }
    public bool IsActive { get; set; }
    public bool IsPublic { get; set; }
    public int SortOrder { get; set; }
    public Guid ApplicationId { get; set; }
    public Guid? ParentPageId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation properties
    public string? ApplicationName { get; set; }
    public string? ParentPageName { get; set; }
    public List<ApplicationPageDto> ChildPages { get; set; } = new();
}

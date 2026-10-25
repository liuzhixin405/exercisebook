using AiDevPlatform.Shared.DTOs.Application;
using AiDevPlatform.Shared.DTOs.User;

namespace AiDevPlatform.Shared.DTOs.Component;

public class ComponentUsageDto
{
    public Guid Id { get; set; }
    public string UsageId { get; set; } = string.Empty;
    public string? Properties { get; set; }
    public string? Styles { get; set; }
    public string? Events { get; set; }
    public string? DataBinding { get; set; }
    public int SortOrder { get; set; }
    public bool IsVisible { get; set; }
    public Guid ComponentId { get; set; }
    public Guid? ApplicationId { get; set; }
    public Guid? PageId { get; set; }
    public Guid? ParentUsageId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation properties
    public ComponentDto? Component { get; set; }
    public ApplicationDto? Application { get; set; }
    public ApplicationPageDto? Page { get; set; }
}

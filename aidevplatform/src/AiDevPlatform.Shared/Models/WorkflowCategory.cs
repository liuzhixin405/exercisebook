using System.ComponentModel.DataAnnotations;

namespace AiDevPlatform.Shared.Models;

public class WorkflowCategory : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [MaxLength(50)]
    public string? Icon { get; set; }

    [MaxLength(20)]
    public string? Color { get; set; }

    public int SortOrder { get; set; } = 0;

    public bool IsActive { get; set; } = true;

    public Guid? ParentId { get; set; }

    // Navigation properties
    public virtual WorkflowCategory? Parent { get; set; }

    public virtual ICollection<WorkflowCategory> Children { get; set; } = new List<WorkflowCategory>();

    public virtual ICollection<Workflow> Workflows { get; set; } = new List<Workflow>();
}

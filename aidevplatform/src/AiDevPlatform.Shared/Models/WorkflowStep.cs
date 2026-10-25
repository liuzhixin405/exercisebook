using System.ComponentModel.DataAnnotations;

namespace AiDevPlatform.Shared.Models;

public class WorkflowStep : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(100)]
    public string StepId { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string StepType { get; set; } = string.Empty; // Start, End, Task, Decision, etc.

    public string? Properties { get; set; } // JSON step properties

    public string? Inputs { get; set; } // JSON step inputs

    public string? Outputs { get; set; } // JSON step outputs

    public int SortOrder { get; set; } = 0;

    public bool IsActive { get; set; } = true;

    public Guid WorkflowId { get; set; }

    // Navigation properties
    public virtual Workflow Workflow { get; set; } = null!;
}

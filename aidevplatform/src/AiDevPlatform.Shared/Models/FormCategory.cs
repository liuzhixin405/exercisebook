using System.ComponentModel.DataAnnotations;

namespace AiDevPlatform.Shared.Models;

public class FormCategory : BaseEntity
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
    public virtual FormCategory? Parent { get; set; }

    public virtual ICollection<FormCategory> Children { get; set; } = new List<FormCategory>();

    public virtual ICollection<Form> Forms { get; set; } = new List<Form>();
}

using System.ComponentModel.DataAnnotations;

namespace AiDevPlatform.Shared.Models;

public class ScheduledReport : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string ScheduleName { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(50)]
    public string ScheduleType { get; set; } = string.Empty; // Daily, Weekly, Monthly, Custom

    [MaxLength(200)]
    public string? CronExpression { get; set; } // For custom schedules

    public string? Parameters { get; set; } // JSON execution parameters

    public string? Filters { get; set; } // JSON filters

    public string? Sorting { get; set; } // JSON sorting

    public string? Grouping { get; set; } // JSON grouping

    public string? Aggregations { get; set; } // JSON aggregations

    [MaxLength(50)]
    public string ExportFormat { get; set; } = "PDF";

    public string? EmailRecipients { get; set; } // JSON email recipients

    public bool IsActive { get; set; } = true;

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime? LastExecutedAt { get; set; }

    public DateTime? NextExecutionAt { get; set; }

    public Guid ReportId { get; set; }

    // Navigation properties
    public virtual Report Report { get; set; } = null!;
}

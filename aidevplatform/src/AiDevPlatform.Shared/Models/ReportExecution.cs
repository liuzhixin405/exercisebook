using System.ComponentModel.DataAnnotations;

namespace AiDevPlatform.Shared.Models;

public class ReportExecution : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string ExecutionId { get; set; } = string.Empty;

    public ReportExecutionStatus Status { get; set; } = ReportExecutionStatus.Running;

    public string? Parameters { get; set; } // JSON execution parameters

    public string? Filters { get; set; } // JSON applied filters

    public string? Sorting { get; set; } // JSON applied sorting

    public string? Grouping { get; set; } // JSON applied grouping

    public string? Aggregations { get; set; } // JSON applied aggregations

    public string? ExportFormat { get; set; }

    public string? ExportPath { get; set; }

    public long? RecordCount { get; set; }

    public long? ExecutionTimeMs { get; set; }

    public string? ErrorMessage { get; set; }

    // Alias property for compatibility
    public string? Result => ExportPath;

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public Guid ReportId { get; set; }

    public Guid? ExecutedBy { get; set; }

    // Navigation properties
    public virtual Report Report { get; set; } = null!;

    public virtual User? ExecutedByUser { get; set; }
}

public enum ReportExecutionStatus
{
    Running,
    Completed,
    Failed,
    Cancelled
}

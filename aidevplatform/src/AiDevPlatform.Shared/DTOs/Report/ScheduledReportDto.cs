namespace AiDevPlatform.Shared.DTOs.Report;

public class ScheduledReportDto
{
    public Guid Id { get; set; }
    public string ScheduleName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ScheduleType { get; set; } = string.Empty;
    public string? CronExpression { get; set; }
    public string? Parameters { get; set; }
    public string? Filters { get; set; }
    public string? Sorting { get; set; }
    public string? Grouping { get; set; }
    public string? Aggregations { get; set; }
    public string ExportFormat { get; set; } = "PDF";
    public string? EmailRecipients { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? LastExecutedAt { get; set; }
    public DateTime? NextExecutionAt { get; set; }
    public Guid ReportId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation properties
    public ReportDto? Report { get; set; }
}

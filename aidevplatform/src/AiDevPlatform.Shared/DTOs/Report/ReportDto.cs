using System.ComponentModel.DataAnnotations;

namespace AiDevPlatform.Shared.DTOs.Report;

public class ReportDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Version { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string? Query { get; set; }

    public string? Parameters { get; set; }

    public string? Layout { get; set; }

    public string? Styles { get; set; }

    public string? Filters { get; set; }

    public string? Sorting { get; set; }

    public string? Grouping { get; set; }

    public string? Aggregations { get; set; }

    public string? ExportFormats { get; set; }

    public bool IsActive { get; set; }

    public bool IsSystem { get; set; }

    public bool IsPublic { get; set; }

    public Guid ApplicationId { get; set; }

    public string? ApplicationName { get; set; }

    public Guid? DataModelId { get; set; }

    public string? DataModelName { get; set; }

    public Guid? CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int ExecutionCount { get; set; }
}

public class CreateReportDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    [MaxLength(100)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(20)]
    public string Version { get; set; } = "1.0.0";

    public string Type { get; set; } = "Table";

    public string? Query { get; set; }

    public string? Parameters { get; set; }

    public string? Layout { get; set; }

    public string? Styles { get; set; }

    public string? Filters { get; set; }

    public string? Sorting { get; set; }

    public string? Grouping { get; set; }

    public string? Aggregations { get; set; }

    public string? ExportFormats { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsPublic { get; set; } = false;

    public Guid ApplicationId { get; set; }

    public Guid? DataModelId { get; set; }

    public Guid? CategoryId { get; set; }
}

public class UpdateReportDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [MaxLength(20)]
    public string Version { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string? Query { get; set; }

    public string? Parameters { get; set; }

    public string? Layout { get; set; }

    public string? Styles { get; set; }

    public string? Filters { get; set; }

    public string? Sorting { get; set; }

    public string? Grouping { get; set; }

    public string? Aggregations { get; set; }

    public string? ExportFormats { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsPublic { get; set; } = false;

    public Guid? DataModelId { get; set; }

    public Guid? CategoryId { get; set; }
}

public class ReportExecutionDto
{
    public Guid Id { get; set; }

    public string ExecutionId { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string? Parameters { get; set; }

    public string? Filters { get; set; }

    public string? Sorting { get; set; }

    public string? Grouping { get; set; }

    public string? Aggregations { get; set; }

    public string? ExportFormat { get; set; }

    public string? ExportPath { get; set; }

    public long? RecordCount { get; set; }

    public long? ExecutionTimeMs { get; set; }

    public string? ErrorMessage { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public Guid ReportId { get; set; }

    public string? ReportName { get; set; }

    public Guid? ExecutedBy { get; set; }

    public string? ExecutedByName { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}

public class ExecuteReportDto
{
    public string? Parameters { get; set; }

    public string? Filters { get; set; }

    public string? Sorting { get; set; }

    public string? Grouping { get; set; }

    public string? Aggregations { get; set; }

    public string? ExportFormat { get; set; }
}

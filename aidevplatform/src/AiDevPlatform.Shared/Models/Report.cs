using System.ComponentModel.DataAnnotations;

namespace AiDevPlatform.Shared.Models;

public class Report : BaseEntity
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

    public ReportStatus Status { get; set; } = ReportStatus.Draft;

    public ReportType Type { get; set; } = ReportType.Table;

    public string? Query { get; set; } // SQL query or data source configuration

    public string? Parameters { get; set; } // JSON parameter definitions

    public string? Layout { get; set; } // JSON report layout

    public string? Styles { get; set; } // JSON report styles

    public string? Filters { get; set; } // JSON filter definitions

    public string? Sorting { get; set; } // JSON sorting definitions

    public string? Grouping { get; set; } // JSON grouping definitions

    public string? Aggregations { get; set; } // JSON aggregation definitions

    public string? ExportFormats { get; set; } // JSON export format configurations

    public bool IsActive { get; set; } = true;

    public bool IsSystem { get; set; } = false;

    public bool IsPublic { get; set; } = false;

    public Guid ApplicationId { get; set; }

    public Guid? DataModelId { get; set; }

    public Guid? CategoryId { get; set; }

    // Navigation properties
    public virtual Application Application { get; set; } = null!;

    public virtual DataModel? DataModel { get; set; }

    public virtual ReportCategory? Category { get; set; }

    public virtual ICollection<ReportExecution> Executions { get; set; } = new List<ReportExecution>();

    public virtual ICollection<ReportPermission> Permissions { get; set; } = new List<ReportPermission>();
}

public enum ReportStatus
{
    Draft,
    Published,
    Archived,
    Deleted
}

public enum ReportType
{
    Table,
    Chart,
    Dashboard,
    Label,
    Document
}

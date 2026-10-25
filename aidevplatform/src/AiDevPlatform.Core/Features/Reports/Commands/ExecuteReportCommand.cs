using MediatR;
using AiDevPlatform.Shared.Common;
using AiDevPlatform.Shared.Models;

namespace AiDevPlatform.Core.Features.Reports.Commands;

public class ExecuteReportCommand : IRequest<CommandResult<ReportExecutionResult>>
{
    public Guid ReportId { get; set; }
    public string? Parameters { get; set; } // JSON execution parameters
    public string? Filters { get; set; } // JSON filters
    public string? Sorting { get; set; } // JSON sorting
    public string? Grouping { get; set; } // JSON grouping
    public string? Aggregations { get; set; } // JSON aggregations
    public string? ExportFormat { get; set; } // PDF, Excel, CSV, etc.
    public Guid ExecutedBy { get; set; }
}

public class ReportExecutionResult
{
    public Guid ExecutionId { get; set; }
    public string ExecutionNumber { get; set; } = string.Empty;
    public ReportExecutionStatus Status { get; set; }
    public string? Message { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public long? RecordCount { get; set; }
    public long? ExecutionTimeMs { get; set; }
    public string? ExportPath { get; set; }
    public List<string> Errors { get; set; } = new();
}

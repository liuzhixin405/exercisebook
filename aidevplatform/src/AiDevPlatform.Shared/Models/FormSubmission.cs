using System.ComponentModel.DataAnnotations;

namespace AiDevPlatform.Shared.Models;

public class FormSubmission : BaseEntity
{
    [Required]
    [MaxLength(100)]
    public string SubmissionId { get; set; } = string.Empty;

    public FormSubmissionStatus Status { get; set; } = FormSubmissionStatus.Draft;

    public string? Data { get; set; } // JSON form data

    public string? ValidationErrors { get; set; } // JSON validation errors

    public string? BusinessRuleResults { get; set; } // JSON business rule results

    public DateTime? SubmittedAt { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public DateTime? RejectedAt { get; set; }

    public string? RejectionReason { get; set; }

    public string? Comments { get; set; }

    public string? Attachments { get; set; } // JSON attachment information

    public Guid FormId { get; set; }

    public Guid? SubmittedBy { get; set; }

    public Guid? ApprovedBy { get; set; }

    public Guid? RejectedBy { get; set; }

    // Navigation properties
    public virtual Form Form { get; set; } = null!;

    public virtual User? SubmittedByUser { get; set; }

    public virtual User? ApprovedByUser { get; set; }

    public virtual User? RejectedByUser { get; set; }
}

public enum FormSubmissionStatus
{
    Draft,
    Submitted,
    Approved,
    Rejected,
    Cancelled
}

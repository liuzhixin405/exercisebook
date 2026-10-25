using System.ComponentModel.DataAnnotations;

namespace AiDevPlatform.Shared.DTOs.Form;

public class FormDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Version { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string? Layout { get; set; }

    public string? Fields { get; set; }

    public string? ValidationRules { get; set; }

    public string? BusinessRules { get; set; }

    public string? Styles { get; set; }

    public string? Scripts { get; set; }

    public string? Permissions { get; set; }

    public bool IsActive { get; set; }

    public bool IsSystem { get; set; }

    public Guid ApplicationId { get; set; }

    public string? ApplicationName { get; set; }

    public Guid? DataModelId { get; set; }

    public string? DataModelName { get; set; }

    public Guid? CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int SubmissionCount { get; set; }
}

public class CreateFormDto
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

    public string? Layout { get; set; }

    public string? Fields { get; set; }

    public string? ValidationRules { get; set; }

    public string? BusinessRules { get; set; }

    public string? Styles { get; set; }

    public string? Scripts { get; set; }

    public string? Permissions { get; set; }

    public bool IsActive { get; set; } = true;

    public Guid ApplicationId { get; set; }

    public Guid? DataModelId { get; set; }

    public Guid? CategoryId { get; set; }
}

public class UpdateFormDto
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [MaxLength(20)]
    public string Version { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string? Layout { get; set; }

    public string? Fields { get; set; }

    public string? ValidationRules { get; set; }

    public string? BusinessRules { get; set; }

    public string? Styles { get; set; }

    public string? Scripts { get; set; }

    public string? Permissions { get; set; }

    public bool IsActive { get; set; } = true;

    public Guid? DataModelId { get; set; }

    public Guid? CategoryId { get; set; }
}

public class FormSubmissionDto
{
    public Guid Id { get; set; }

    public string SubmissionId { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string? Data { get; set; }

    public string? ValidationErrors { get; set; }

    public string? BusinessRuleResults { get; set; }

    public DateTime? SubmittedAt { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public DateTime? RejectedAt { get; set; }

    public string? RejectionReason { get; set; }

    public string? Comments { get; set; }

    public string? Attachments { get; set; }

    public Guid FormId { get; set; }

    public string? FormName { get; set; }

    public Guid? SubmittedBy { get; set; }

    public string? SubmittedByName { get; set; }

    public Guid? ApprovedBy { get; set; }

    public string? ApprovedByName { get; set; }

    public Guid? RejectedBy { get; set; }

    public string? RejectedByName { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}

public class SubmitFormDto
{
    public string? Data { get; set; }

    public string? Comments { get; set; }

    public string? Attachments { get; set; }
}

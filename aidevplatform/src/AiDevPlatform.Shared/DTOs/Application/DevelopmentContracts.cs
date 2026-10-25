namespace AiDevPlatform.Shared.DTOs.Application;

public sealed class TeamStandard
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public List<string> RequiredFiles { get; set; } = new();
    public List<string> ForbiddenTerms { get; set; } = new();
    public List<string> RequiredChecks { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public sealed class SourceFile
{
    public string Path { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

public sealed class CreateDevelopmentTask
{
    public string Title { get; set; } = string.Empty;
    public string Technology { get; set; } = string.Empty;
    public string Requirement { get; set; } = string.Empty;
    public Guid StandardId { get; set; }
    public List<SourceFile> ContextFiles { get; set; } = new();
}

public sealed class RequirementDraft
{
    public string Specification { get; set; } = string.Empty;
    public List<string> Questions { get; set; } = new();
    public List<string> Assumptions { get; set; } = new();
    public List<string> AcceptanceCriteria { get; set; } = new();
    public List<string> PlannedFiles { get; set; } = new();
}

public sealed class ConfirmDevelopmentTask
{
    public int ExpectedRevision { get; set; }
    public RequirementDraft Draft { get; set; } = new();
    public List<string> Answers { get; set; } = new();
}

public sealed class DevelopmentAction
{
    public int ExpectedRevision { get; set; }
    public string GenerationMode { get; set; } = "change";
    public string TargetPath { get; set; } = string.Empty;
    public string Feedback { get; set; } = string.Empty;
    public string Decision { get; set; } = string.Empty;
    public bool RiskAcknowledged { get; set; }
}

public sealed class QualityCheck
{
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
}

public sealed class ChangeRevision
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Summary { get; set; } = string.Empty;
    public string GenerationFeedback { get; set; } = string.Empty;
    public string GenerationTargetPath { get; set; } = string.Empty;
    public List<SourceFile> Files { get; set; } = new();
    public List<QualityCheck> Checks { get; set; } = new();
    public List<VerificationReport> Verifications { get; set; } = new();
    public AiGenerationTrace? Generation { get; set; }
    public ProjectWorkspace? Workspace { get; set; }
    public string Review { get; set; } = "pending";
    public string ReviewComment { get; set; } = string.Empty;
}

public sealed class DevelopmentTask
{
    public Guid Id { get; set; }
    public int Revision { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Technology { get; set; } = string.Empty;
    public string Requirement { get; set; } = string.Empty;
    public TeamStandard Standard { get; set; } = new();
    public RequirementDraft? Draft { get; set; }
    public List<string> Answers { get; set; } = new();
    public bool Confirmed { get; set; }
    public List<SourceFile> ContextFiles { get; set; } = new();
    public List<ChangeRevision> Changes { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}

public sealed class AiChangeProposal
{
    public string Summary { get; set; } = string.Empty;
    public List<SourceFile> Files { get; set; } = new();
    public AiGenerationTrace? Generation { get; set; }
}

public sealed class AiGenerationTrace
{
    public Guid RequestId { get; set; }
    public string Model { get; set; } = string.Empty;
    public DateTime CompletedAt { get; set; }
}

public sealed class ProjectWorkspace
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public Guid ChangeId { get; set; }
    public string DirectoryPath { get; set; } = string.Empty;
    public string RuntimeStatus { get; set; } = "not-run";
    public DateTime CreatedAt { get; set; }
    public List<ProjectFileFingerprint> Files { get; set; } = new();
}

public sealed class ProjectFileFingerprint
{
    public string Path { get; set; } = string.Empty;
    public string Sha256 { get; set; } = string.Empty;
    public int Utf8Bytes { get; set; }
    public string Origin { get; set; } = string.Empty;
}

public sealed class VerificationRequest
{
    public Guid RunId { get; set; }
    public Guid TaskId { get; set; }
    public Guid ChangeId { get; set; }
    public string Technology { get; set; } = string.Empty;
    public List<SourceFile> Files { get; set; } = new();
    public List<string> RequiredChecks { get; set; } = new();
}

public sealed class VerificationReport
{
    public Guid RunId { get; set; }
    public Guid TaskId { get; set; }
    public Guid ChangeId { get; set; }
    public DateTime ReceivedAt { get; set; }
    public List<QualityCheck> Checks { get; set; } = new();
}
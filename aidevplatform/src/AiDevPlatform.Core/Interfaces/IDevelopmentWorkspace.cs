using AiDevPlatform.Shared.DTOs.Application;

namespace AiDevPlatform.Core.Interfaces;

public interface IDevelopmentStore
{
    Task<List<TeamStandard>> StandardsAsync(CancellationToken cancellationToken);
    Task AddStandardAsync(TeamStandard standard, CancellationToken cancellationToken);
    Task<List<DevelopmentTask>> TasksAsync(CancellationToken cancellationToken);
    Task<DevelopmentTask?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task SaveAsync(DevelopmentTask task, int expectedRevision, CancellationToken cancellationToken);
    Task<ProjectWorkspace> MaterializeAsync(DevelopmentTask task, ChangeRevision change, CancellationToken cancellationToken);
}

public interface IAiDevelopmentGenerator
{
    Task<RequirementDraft> ClarifyAsync(DevelopmentTask task, CancellationToken cancellationToken);
    Task<AiChangeProposal> GenerateAsync(DevelopmentTask task, string feedback, CancellationToken cancellationToken);
    Task<AiChangeProposal> GenerateFilesAsync(DevelopmentTask task, List<string> paths, string feedback, CancellationToken cancellationToken);
}

public interface IDevelopmentVerifier
{
    Task<VerificationReport> VerifyAsync(VerificationRequest request, CancellationToken cancellationToken);
}
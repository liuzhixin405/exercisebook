using AiDevPlatform.Shared.DTOs.ApplicationPage;

namespace AiDevPlatform.Core.Interfaces;

public interface IPageDraftGenerator
{
    Task<PageDraft> GenerateAsync(string prompt, CancellationToken cancellationToken = default);
}
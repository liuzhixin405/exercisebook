using AiDevPlatform.Shared.Models;

namespace AiDevPlatform.Core.Interfaces;

public interface ICodeTemplateRepository : IRepository<CodeTemplate>
{
    Task<bool> NameExistsAsync(string name, Guid? applicationId = null, Guid? excludeId = null);
    Task<IEnumerable<CodeTemplate>> GetByTypeAsync(string templateType);
    Task<IEnumerable<CodeTemplate>> GetApplicationTemplatesAsync(Guid applicationId);
    Task<IEnumerable<CodeTemplate>> GetSystemTemplatesAsync();
}

using AiDevPlatform.Shared.Models;
using AiDevPlatform.Shared.DTOs.Common;

namespace AiDevPlatform.Core.Interfaces;

public interface IComponentRepository : IRepository<Component>
{
    Task<PagedResult<Component>> GetPagedComponentsAsync(PagedRequest request);
    Task<IEnumerable<Component>> GetComponentsByCategoryAsync(ComponentCategory category);
    Task<IEnumerable<Component>> GetComponentsByTypeAsync(ComponentType type);
    Task<IEnumerable<Component>> GetPublicComponentsAsync();
    Task<IEnumerable<Component>> GetUserComponentsAsync(Guid userId);
    Task<IEnumerable<Component>> GetApplicationComponentsAsync(Guid applicationId);
    Task<Component?> GetByCodeAsync(string code);
    Task<bool> CodeExistsAsync(string code, Guid? excludeId = null);
    Task UpdateUsageCountAsync(Guid componentId);
}

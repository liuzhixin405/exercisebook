using AiDevPlatform.Shared.Models;
using AiDevPlatform.Shared.DTOs.Common;

namespace AiDevPlatform.Core.Interfaces;

public interface IApplicationRepository : IRepository<Application>
{
    Task<PagedResult<Application>> GetPagedApplicationsAsync(PagedRequest request, string userId);
    Task<Application?> GetApplicationWithDetailsAsync(Guid id, string userId);
    Task<IEnumerable<Application>> GetUserApplicationsAsync(string userId);
    Task<IEnumerable<Application>> GetOrganizationApplicationsAsync(Guid organizationId);
    Task<bool> IsApplicationOwnerAsync(Guid applicationId, string userId);
    Task<bool> HasApplicationAccessAsync(Guid applicationId, string userId);
    Task<Application?> GetByCodeAsync(string code);
    Task<bool> CodeExistsAsync(string code, Guid? excludeId = null);
}

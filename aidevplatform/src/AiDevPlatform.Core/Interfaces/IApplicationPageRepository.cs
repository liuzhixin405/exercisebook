using AiDevPlatform.Shared.Models;

namespace AiDevPlatform.Core.Interfaces;

public interface IApplicationPageRepository : IRepository<ApplicationPage>
{
    Task<IEnumerable<ApplicationPage>> GetApplicationPagesAsync(Guid applicationId);
    Task<ApplicationPage?> GetPageByRouteAsync(Guid applicationId, string route);
    Task<IEnumerable<ApplicationPage>> GetChildPagesAsync(Guid parentPageId);
    Task<bool> RouteExistsAsync(Guid applicationId, string route, Guid? excludePageId = null);
    Task<IEnumerable<ApplicationPage>> GetPublicPagesAsync(Guid applicationId);
    Task<IEnumerable<ApplicationPage>> GetActivePagesAsync(Guid applicationId);
}

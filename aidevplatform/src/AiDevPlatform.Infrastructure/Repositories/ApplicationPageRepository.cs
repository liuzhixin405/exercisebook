using Microsoft.EntityFrameworkCore;
using AiDevPlatform.Core.Interfaces;
using AiDevPlatform.Infrastructure.Data;
using AiDevPlatform.Shared.Models;

namespace AiDevPlatform.Infrastructure.Repositories;

public class ApplicationPageRepository : Repository<ApplicationPage>, IApplicationPageRepository
{
    public ApplicationPageRepository(AiDevPlatformDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ApplicationPage>> GetApplicationPagesAsync(Guid applicationId)
    {
        return await _dbSet
            .Where(p => p.ApplicationId == applicationId && !p.IsDeleted)
            .OrderBy(p => p.SortOrder)
            .ThenBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<ApplicationPage?> GetPageByRouteAsync(Guid applicationId, string route)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.ApplicationId == applicationId && p.Route == route && !p.IsDeleted);
    }

    public async Task<IEnumerable<ApplicationPage>> GetChildPagesAsync(Guid parentPageId)
    {
        return await _dbSet
            .Where(p => p.ParentPageId == parentPageId && !p.IsDeleted)
            .OrderBy(p => p.SortOrder)
            .ThenBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<bool> RouteExistsAsync(Guid applicationId, string route, Guid? excludePageId = null)
    {
        var query = _dbSet.Where(p => p.ApplicationId == applicationId && p.Route == route && !p.IsDeleted);

        if (excludePageId.HasValue)
        {
            query = query.Where(p => p.Id != excludePageId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<IEnumerable<ApplicationPage>> GetPublicPagesAsync(Guid applicationId)
    {
        return await _dbSet
            .Where(p => p.ApplicationId == applicationId && p.IsPublic && p.IsActive && !p.IsDeleted)
            .OrderBy(p => p.SortOrder)
            .ThenBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<ApplicationPage>> GetActivePagesAsync(Guid applicationId)
    {
        return await _dbSet
            .Where(p => p.ApplicationId == applicationId && p.IsActive && !p.IsDeleted)
            .OrderBy(p => p.SortOrder)
            .ThenBy(p => p.Name)
            .ToListAsync();
    }
}

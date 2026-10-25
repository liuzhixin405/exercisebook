using Microsoft.EntityFrameworkCore;
using AiDevPlatform.Core.Interfaces;
using AiDevPlatform.Shared.Models;
using AiDevPlatform.Shared.DTOs.Common;
using AiDevPlatform.Infrastructure.Data;

namespace AiDevPlatform.Infrastructure.Repositories;

public class ApplicationRepository : Repository<Application>, IApplicationRepository
{
    public ApplicationRepository(AiDevPlatformDbContext context) : base(context)
    {
    }

    public async Task<PagedResult<Application>> GetPagedApplicationsAsync(PagedRequest request, string userId)
    {
        IQueryable<Application> query = _dbSet
            .Where(a => !a.IsDeleted)
            .Include(a => a.Owner)
            .Include(a => a.Organization)
            .Include(a => a.Pages)
            .Include(a => a.Forms)
            .Include(a => a.Workflows)
            .Include(a => a.Reports);

        // Filter by user access
        query = query.Where(a => a.OwnerId.ToString() == userId ||
                                a.ApplicationUsers.Any(au => au.UserId.ToString() == userId));

        // Apply search filter
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(a => a.Name.Contains(request.SearchTerm) ||
                                   a.Description!.Contains(request.SearchTerm) ||
                                   a.Code.Contains(request.SearchTerm));
        }

        // Apply additional filters
        if (request.Filters != null)
        {
            foreach (var filter in request.Filters)
            {
                switch (filter.Key.ToLower())
                {
                    case "status":
                        if (Enum.TryParse<ApplicationStatus>(filter.Value.ToString(), out var status))
                        {
                            query = query.Where(a => a.Status == status);
                        }
                        break;
                    case "type":
                        if (Enum.TryParse<ApplicationType>(filter.Value.ToString(), out var type))
                        {
                            query = query.Where(a => a.Type == type);
                        }
                        break;
                    case "category":
                        query = query.Where(a => a.Category == filter.Value.ToString());
                        break;
                }
            }
        }

        // Apply sorting
        if (!string.IsNullOrEmpty(request.SortBy))
        {
            switch (request.SortBy.ToLower())
            {
                case "name":
                    query = request.SortDescending ? query.OrderByDescending(a => a.Name) : query.OrderBy(a => a.Name);
                    break;
                case "createdat":
                    query = request.SortDescending ? query.OrderByDescending(a => a.CreatedAt) : query.OrderBy(a => a.CreatedAt);
                    break;
                case "updatedat":
                    query = request.SortDescending ? query.OrderByDescending(a => a.UpdatedAt) : query.OrderBy(a => a.UpdatedAt);
                    break;
                default:
                    query = query.OrderByDescending(a => a.CreatedAt);
                    break;
            }
        }
        else
        {
            query = query.OrderByDescending(a => a.CreatedAt);
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return PagedResult<Application>.Create(items, totalCount, request.PageNumber, request.PageSize);
    }

    public async Task<Application?> GetApplicationWithDetailsAsync(Guid id, string userId)
    {
        var application = await _dbSet
            .Where(a => !a.IsDeleted && a.Id == id)
            .Include(a => a.Owner)
            .Include(a => a.Organization)
            .Include(a => a.Pages)
            .Include(a => a.Forms)
            .Include(a => a.Workflows)
            .Include(a => a.Reports)
            .Include(a => a.DataModels)
            .Include(a => a.ApplicationUsers)
            .ThenInclude(au => au.User)
            .FirstOrDefaultAsync();

        if (application == null)
            return null;

        // Check if user has access
        if (application.OwnerId.ToString() != userId &&
            !application.ApplicationUsers.Any(au => au.UserId.ToString() == userId))
        {
            return null;
        }

        return application;
    }

    public async Task<IEnumerable<Application>> GetUserApplicationsAsync(string userId)
    {
        return await _dbSet
            .Where(a => !a.IsDeleted &&
                       (a.OwnerId.ToString() == userId ||
                        a.ApplicationUsers.Any(au => au.UserId.ToString() == userId)))
            .Include(a => a.Owner)
            .Include(a => a.Organization)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Application>> GetOrganizationApplicationsAsync(Guid organizationId)
    {
        return await _dbSet
            .Where(a => !a.IsDeleted && a.OrganizationId == organizationId)
            .Include(a => a.Owner)
            .Include(a => a.Organization)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> IsApplicationOwnerAsync(Guid applicationId, string userId)
    {
        return await _dbSet
            .Where(a => !a.IsDeleted && a.Id == applicationId && a.OwnerId.ToString() == userId)
            .AnyAsync();
    }

    public async Task<bool> HasApplicationAccessAsync(Guid applicationId, string userId)
    {
        return await _dbSet
            .Where(a => !a.IsDeleted && a.Id == applicationId &&
                       (a.OwnerId.ToString() == userId ||
                        a.ApplicationUsers.Any(au => au.UserId.ToString() == userId)))
            .AnyAsync();
    }

    public async Task<Application?> GetByCodeAsync(string code)
    {
        return await _dbSet
            .Where(a => !a.IsDeleted && a.Code == code)
            .Include(a => a.Owner)
            .Include(a => a.Organization)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> CodeExistsAsync(string code, Guid? excludeId = null)
    {
        var query = _dbSet.Where(a => !a.IsDeleted && a.Code == code);

        if (excludeId.HasValue)
        {
            query = query.Where(a => a.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }
}

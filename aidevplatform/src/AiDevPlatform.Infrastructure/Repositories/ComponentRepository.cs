using Microsoft.EntityFrameworkCore;
using AiDevPlatform.Core.Interfaces;
using AiDevPlatform.Shared.Models;
using AiDevPlatform.Shared.DTOs.Common;
using AiDevPlatform.Infrastructure.Data;

namespace AiDevPlatform.Infrastructure.Repositories;

public class ComponentRepository : Repository<Component>, IComponentRepository
{
    public ComponentRepository(AiDevPlatformDbContext context) : base(context)
    {
    }

    public async Task<PagedResult<Component>> GetPagedComponentsAsync(PagedRequest request)
    {
        IQueryable<Component> query = _dbSet
            .Where(c => !c.IsDeleted)
            .Include(c => c.Application)
            .Include(c => c.CreatedBy);

        // Apply search filter
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(c => c.Name.Contains(request.SearchTerm) ||
                                   c.Description!.Contains(request.SearchTerm) ||
                                   c.Code.Contains(request.SearchTerm));
        }

        // Apply additional filters
        if (request.Filters != null)
        {
            foreach (var filter in request.Filters)
            {
                switch (filter.Key.ToLower())
                {
                    case "type":
                        if (Enum.TryParse<ComponentType>(filter.Value.ToString(), out var type))
                        {
                            query = query.Where(c => c.Type == type);
                        }
                        break;
                    case "category":
                        if (Enum.TryParse<ComponentCategory>(filter.Value.ToString(), out var category))
                        {
                            query = query.Where(c => c.Category == category);
                        }
                        break;
                    case "ispublic":
                        if (bool.TryParse(filter.Value.ToString(), out var isPublic))
                        {
                            query = query.Where(c => c.IsPublic == isPublic);
                        }
                        break;
                    case "isactive":
                        if (bool.TryParse(filter.Value.ToString(), out var isActive))
                        {
                            query = query.Where(c => c.IsActive == isActive);
                        }
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
                    query = request.SortDescending ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name);
                    break;
                case "createdat":
                    query = request.SortDescending ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt);
                    break;
                case "updatedat":
                    query = request.SortDescending ? query.OrderByDescending(c => c.UpdatedAt) : query.OrderBy(c => c.UpdatedAt);
                    break;
                default:
                    query = query.OrderByDescending(c => c.CreatedAt);
                    break;
            }
        }
        else
        {
            query = query.OrderByDescending(c => c.CreatedAt);
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return PagedResult<Component>.Create(items, totalCount, request.PageNumber, request.PageSize);
    }

    public async Task<IEnumerable<Component>> GetComponentsByCategoryAsync(ComponentCategory category)
    {
        return await _dbSet
            .Where(c => !c.IsDeleted && c.Category == category && c.IsActive)
            .Include(c => c.Application)
            .Include(c => c.CreatedBy)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Component>> GetComponentsByTypeAsync(ComponentType type)
    {
        return await _dbSet
            .Where(c => !c.IsDeleted && c.Type == type && c.IsActive)
            .Include(c => c.Application)
            .Include(c => c.CreatedBy)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Component>> GetPublicComponentsAsync()
    {
        return await _dbSet
            .Where(c => !c.IsDeleted && c.IsPublic && c.IsActive)
            .Include(c => c.Application)
            .Include(c => c.CreatedBy)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Component>> GetUserComponentsAsync(Guid userId)
    {
        return await _dbSet
            .Where(c => !c.IsDeleted && c.CreatedById == userId)
            .Include(c => c.Application)
            .Include(c => c.CreatedBy)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Component>> GetApplicationComponentsAsync(Guid applicationId)
    {
        return await _dbSet
            .Where(c => !c.IsDeleted && c.ApplicationId == applicationId)
            .Include(c => c.Application)
            .Include(c => c.CreatedBy)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Component?> GetByCodeAsync(string code)
    {
        return await _dbSet
            .Where(c => !c.IsDeleted && c.Code == code)
            .Include(c => c.Application)
            .Include(c => c.CreatedBy)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> CodeExistsAsync(string code, Guid? excludeId = null)
    {
        var query = _dbSet.Where(c => !c.IsDeleted && c.Code == code);

        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task UpdateUsageCountAsync(Guid componentId)
    {
        var component = await _dbSet.FindAsync(componentId);
        if (component != null)
        {
            // This would need to be implemented based on your usage tracking logic
            component.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}

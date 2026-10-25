using Microsoft.EntityFrameworkCore;
using AiDevPlatform.Core.Interfaces;
using AiDevPlatform.Shared.Models;
using AiDevPlatform.Shared.DTOs.Common;
using AiDevPlatform.Infrastructure.Data;

namespace AiDevPlatform.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AiDevPlatformDbContext _context;
    private readonly DbSet<User> _dbSet;

    public UserRepository(AiDevPlatformDbContext context)
    {
        _context = context;
        _dbSet = context.Set<User>();
    }

    public async Task<User?> GetByUserNameAsync(string userName)
    {
        return await _dbSet
            .Where(u => !u.IsDeleted && u.UserName == userName)
            .FirstOrDefaultAsync();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .Where(u => !u.IsDeleted && u.Email == email)
            .FirstOrDefaultAsync();
    }

    public async Task<User?> GetUserWithRolesAsync(Guid id)
    {
        return await _dbSet
            .Where(u => !u.IsDeleted && u.Id == id)
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync();
    }

    public async Task<User?> GetUserWithPermissionsAsync(Guid id)
    {
        return await _dbSet
            .Where(u => !u.IsDeleted && u.Id == id)
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .Include(u => u.UserPermissions)
            .ThenInclude(up => up.Permission)
            .FirstOrDefaultAsync();
    }

    public async Task<PagedResult<User>> GetPagedUsersAsync(PagedRequest request)
    {
        var query = _dbSet.Where(u => !u.IsDeleted);

        // Apply search filter
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(u => u.UserName!.Contains(request.SearchTerm) ||
                                   u.Email!.Contains(request.SearchTerm) ||
                                   u.FirstName.Contains(request.SearchTerm) ||
                                   u.LastName.Contains(request.SearchTerm));
        }

        // Apply additional filters
        if (request.Filters != null)
        {
            foreach (var filter in request.Filters)
            {
                switch (filter.Key.ToLower())
                {
                    case "isactive":
                        if (bool.TryParse(filter.Value.ToString(), out var isActive))
                        {
                            query = query.Where(u => u.IsActive == isActive);
                        }
                        break;
                    case "language":
                        query = query.Where(u => u.Language == filter.Value.ToString());
                        break;
                }
            }
        }

        // Apply sorting
        if (!string.IsNullOrEmpty(request.SortBy))
        {
            switch (request.SortBy.ToLower())
            {
                case "username":
                    query = request.SortDescending ? query.OrderByDescending(u => u.UserName) : query.OrderBy(u => u.UserName);
                    break;
                case "email":
                    query = request.SortDescending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email);
                    break;
                case "firstname":
                    query = request.SortDescending ? query.OrderByDescending(u => u.FirstName) : query.OrderBy(u => u.FirstName);
                    break;
                case "lastname":
                    query = request.SortDescending ? query.OrderByDescending(u => u.LastName) : query.OrderBy(u => u.LastName);
                    break;
                case "createdat":
                    query = request.SortDescending ? query.OrderByDescending(u => u.CreatedAt) : query.OrderBy(u => u.CreatedAt);
                    break;
                default:
                    query = query.OrderByDescending(u => u.CreatedAt);
                    break;
            }
        }
        else
        {
            query = query.OrderByDescending(u => u.CreatedAt);
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return PagedResult<User>.Create(items, totalCount, request.PageNumber, request.PageSize);
    }

    public async Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
    {
        return await _dbSet
            .Where(u => !u.IsDeleted && u.UserRoles.Any(ur => ur.Role.Name == role))
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetUsersByOrganizationAsync(Guid organizationId)
    {
        return await _dbSet
            .Where(u => !u.IsDeleted && u.OrganizationUsers.Any(ou => ou.OrganizationId == organizationId))
            .Include(u => u.OrganizationUsers)
            .ThenInclude(ou => ou.Organization)
            .ToListAsync();
    }

    public async Task<bool> UserNameExistsAsync(string userName, Guid? excludeId = null)
    {
        var query = _dbSet.Where(u => !u.IsDeleted && u.UserName == userName);

        if (excludeId.HasValue)
        {
            query = query.Where(u => u.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<bool> EmailExistsAsync(string email, Guid? excludeId = null)
    {
        var query = _dbSet.Where(u => !u.IsDeleted && u.Email == email);

        if (excludeId.HasValue)
        {
            query = query.Where(u => u.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task UpdateLastLoginAsync(Guid userId)
    {
        var user = await _dbSet.FindAsync(userId);
        if (user != null)
        {
            user.LastLoginAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    // IRepository<User> implementation
    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _dbSet.Where(u => !u.IsDeleted).ToListAsync();
    }

    public async Task<IEnumerable<User>> FindAsync(System.Linq.Expressions.Expression<Func<User, bool>> predicate)
    {
        return await _dbSet.Where(predicate).Where(u => !u.IsDeleted).ToListAsync();
    }

    public async Task<User?> FirstOrDefaultAsync(System.Linq.Expressions.Expression<Func<User, bool>> predicate)
    {
        return await _dbSet.Where(predicate).Where(u => !u.IsDeleted).FirstOrDefaultAsync();
    }

    public async Task<User> AddAsync(User entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        _dbSet.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<IEnumerable<User>> AddRangeAsync(IEnumerable<User> entities)
    {
        var entityList = entities.ToList();
        foreach (var entity in entityList)
        {
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
        }
        _dbSet.AddRange(entityList);
        await _context.SaveChangesAsync();
        return entityList;
    }

    public async Task UpdateAsync(User entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRangeAsync(IEnumerable<User> entities)
    {
        foreach (var entity in entities)
        {
            entity.UpdatedAt = DateTime.UtcNow;
        }
        _dbSet.UpdateRange(entities);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(User entity)
    {
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteRangeAsync(IEnumerable<User> entities)
    {
        foreach (var entity in entities)
        {
            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;
        }
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(System.Linq.Expressions.Expression<Func<User, bool>> predicate)
    {
        return await _dbSet.Where(predicate).Where(u => !u.IsDeleted).AnyAsync();
    }

    public async Task<int> CountAsync(System.Linq.Expressions.Expression<Func<User, bool>>? predicate = null)
    {
        var query = _dbSet.Where(u => !u.IsDeleted);
        if (predicate != null)
        {
            query = query.Where(predicate);
        }
        return await query.CountAsync();
    }

    public async Task<IEnumerable<User>> GetPagedAsync(int pageNumber, int pageSize, System.Linq.Expressions.Expression<Func<User, bool>>? predicate = null, System.Linq.Expressions.Expression<Func<User, object>>? orderBy = null, bool descending = false)
    {
        var query = _dbSet.Where(u => !u.IsDeleted);

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        if (orderBy != null)
        {
            query = descending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
        }
        else
        {
            query = query.OrderByDescending(u => u.CreatedAt);
        }

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}

using Microsoft.EntityFrameworkCore;
using AiDevPlatform.Core.Interfaces;
using AiDevPlatform.Shared.Models;
using AiDevPlatform.Shared.DTOs.Common;
using AiDevPlatform.Infrastructure.Data;

namespace AiDevPlatform.Infrastructure.Repositories;

public class DeploymentRepository : Repository<Deployment>, IDeploymentRepository
{
    public DeploymentRepository(AiDevPlatformDbContext context) : base(context)
    {
    }

    public async Task<PagedResult<Deployment>> GetPagedDeploymentsAsync(PagedRequest request, Guid applicationId)
    {
        IQueryable<Deployment> query = _dbSet
            .Where(d => !d.IsDeleted && d.ApplicationId == applicationId)
            .Include(d => d.Application)
            .Include(d => d.DeployedByUser);

        // Apply search filter
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(d => d.Name.Contains(request.SearchTerm) ||
                                   d.Description!.Contains(request.SearchTerm) ||
                                   d.Version.Contains(request.SearchTerm));
        }

        // Apply additional filters
        if (request.Filters != null)
        {
            foreach (var filter in request.Filters)
            {
                switch (filter.Key.ToLower())
                {
                    case "status":
                        if (Enum.TryParse<DeploymentStatus>(filter.Value.ToString(), out var status))
                        {
                            query = query.Where(d => d.Status == status);
                        }
                        break;
                    case "environment":
                        query = query.Where(d => d.Environment == filter.Value.ToString());
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
                    query = request.SortDescending ? query.OrderByDescending(d => d.Name) : query.OrderBy(d => d.Name);
                    break;
                case "version":
                    query = request.SortDescending ? query.OrderByDescending(d => d.Version) : query.OrderBy(d => d.Version);
                    break;
                case "status":
                    query = request.SortDescending ? query.OrderByDescending(d => d.Status) : query.OrderBy(d => d.Status);
                    break;
                case "deployedat":
                    query = request.SortDescending ? query.OrderByDescending(d => d.DeployedAt) : query.OrderBy(d => d.DeployedAt);
                    break;
                default:
                    query = query.OrderByDescending(d => d.CreatedAt);
                    break;
            }
        }
        else
        {
            query = query.OrderByDescending(d => d.CreatedAt);
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return PagedResult<Deployment>.Create(items, totalCount, request.PageNumber, request.PageSize);
    }

    public async Task<IEnumerable<Deployment>> GetApplicationDeploymentsAsync(Guid applicationId)
    {
        return await _dbSet
            .Where(d => !d.IsDeleted && d.ApplicationId == applicationId)
            .Include(d => d.Application)
            .Include(d => d.DeployedByUser)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Deployment>> GetDeploymentsByStatusAsync(DeploymentStatus status)
    {
        return await _dbSet
            .Where(d => !d.IsDeleted && d.Status == status)
            .Include(d => d.Application)
            .Include(d => d.DeployedByUser)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();
    }

    public async Task<Deployment?> GetLatestDeploymentAsync(Guid applicationId, string? environment = null)
    {
        var query = _dbSet
            .Where(d => !d.IsDeleted && d.ApplicationId == applicationId && d.Status == DeploymentStatus.Success);

        if (!string.IsNullOrEmpty(environment))
        {
            query = query.Where(d => d.Environment == environment);
        }

        return await query
            .Include(d => d.Application)
            .Include(d => d.DeployedByUser)
            .OrderByDescending(d => d.DeployedAt)
            .FirstOrDefaultAsync();
    }

    public async Task UpdateDeploymentStatusAsync(Guid deploymentId, DeploymentStatus status)
    {
        var deployment = await _dbSet.FindAsync(deploymentId);
        if (deployment != null)
        {
            deployment.Status = status;
            deployment.UpdatedAt = DateTime.UtcNow;

            if (status == DeploymentStatus.Success || status == DeploymentStatus.Failed)
            {
                deployment.CompletedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }
    }

    public async Task AddDeploymentLogAsync(Guid deploymentId, string level, string message, string? details = null)
    {
        // This would typically add to a separate DeploymentLog table
        // For now, we'll append to the DeployLog field
        var deployment = await _dbSet.FindAsync(deploymentId);
        if (deployment != null)
        {
            var logEntry = $"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
            if (!string.IsNullOrEmpty(details))
            {
                logEntry += $"\nDetails: {details}";
            }

            deployment.DeployLog = string.IsNullOrEmpty(deployment.DeployLog)
                ? logEntry
                : deployment.DeployLog + "\n" + logEntry;

            deployment.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}

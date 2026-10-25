using Microsoft.EntityFrameworkCore;
using AiDevPlatform.Core.Interfaces;
using AiDevPlatform.Shared.Models;
using AiDevPlatform.Shared.DTOs.Common;
using AiDevPlatform.Infrastructure.Data;

namespace AiDevPlatform.Infrastructure.Repositories;

public class WorkflowRepository : Repository<Workflow>, IWorkflowRepository
{
    public WorkflowRepository(AiDevPlatformDbContext context) : base(context)
    {
    }

    public async Task<PagedResult<Workflow>> GetPagedWorkflowsAsync(PagedRequest request, Guid applicationId)
    {
        IQueryable<Workflow> query = _dbSet
            .Where(w => !w.IsDeleted && w.ApplicationId == applicationId)
            .Include(w => w.Application)
            .Include(w => w.Category);

        // Apply search filter
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(w => w.Name.Contains(request.SearchTerm) ||
                                   w.Description!.Contains(request.SearchTerm) ||
                                   w.Code.Contains(request.SearchTerm));
        }

        // Apply additional filters
        if (request.Filters != null)
        {
            foreach (var filter in request.Filters)
            {
                switch (filter.Key.ToLower())
                {
                    case "status":
                        if (Enum.TryParse<WorkflowStatus>(filter.Value.ToString(), out var status))
                        {
                            query = query.Where(w => w.Status == status);
                        }
                        break;
                    case "isactive":
                        if (bool.TryParse(filter.Value.ToString(), out var isActive))
                        {
                            query = query.Where(w => w.IsActive == isActive);
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
                    query = request.SortDescending ? query.OrderByDescending(w => w.Name) : query.OrderBy(w => w.Name);
                    break;
                case "createdat":
                    query = request.SortDescending ? query.OrderByDescending(w => w.CreatedAt) : query.OrderBy(w => w.CreatedAt);
                    break;
                default:
                    query = query.OrderByDescending(w => w.CreatedAt);
                    break;
            }
        }
        else
        {
            query = query.OrderByDescending(w => w.CreatedAt);
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return PagedResult<Workflow>.Create(items, totalCount, request.PageNumber, request.PageSize);
    }

    public async Task<IEnumerable<Workflow>> GetApplicationWorkflowsAsync(Guid applicationId)
    {
        return await _dbSet
            .Where(w => !w.IsDeleted && w.ApplicationId == applicationId)
            .Include(w => w.Application)
            .Include(w => w.Category)
            .OrderBy(w => w.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Workflow>> GetActiveWorkflowsAsync()
    {
        return await _dbSet
            .Where(w => !w.IsDeleted && w.IsActive && w.Status == WorkflowStatus.Published)
            .Include(w => w.Application)
            .Include(w => w.Category)
            .OrderBy(w => w.Name)
            .ToListAsync();
    }

    public async Task<Workflow?> GetByCodeAsync(string code)
    {
        return await _dbSet
            .Where(w => !w.IsDeleted && w.Code == code)
            .Include(w => w.Application)
            .Include(w => w.Category)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> CodeExistsAsync(string code, Guid? excludeId = null)
    {
        var query = _dbSet.Where(w => !w.IsDeleted && w.Code == code);

        if (excludeId.HasValue)
        {
            query = query.Where(w => w.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<WorkflowInstance?> GetWorkflowInstanceAsync(string instanceId)
    {
        return await _context.WorkflowInstances
            .Where(wi => !wi.IsDeleted && wi.InstanceId == instanceId)
            .Include(wi => wi.Workflow)
            .Include(wi => wi.StartedByUser)
            .Include(wi => wi.AssignedToUser)
            .Include(wi => wi.Tasks)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<WorkflowInstance>> GetUserWorkflowInstancesAsync(Guid userId)
    {
        return await _context.WorkflowInstances
            .Where(wi => !wi.IsDeleted && (wi.StartedBy == userId || wi.AssignedTo == userId))
            .Include(wi => wi.Workflow)
            .Include(wi => wi.StartedByUser)
            .Include(wi => wi.AssignedToUser)
            .OrderByDescending(wi => wi.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<WorkflowTask>> GetUserTasksAsync(Guid userId)
    {
        return await _context.WorkflowTasks
            .Where(wt => !wt.IsDeleted && wt.AssignedTo == userId && wt.Status == WorkflowTaskStatus.Pending)
            .Include(wt => wt.WorkflowInstance)
            .ThenInclude(wi => wi.Workflow)
            .Include(wt => wt.AssignedToUser)
            .Include(wt => wt.CreatedByUser)
            .OrderBy(wt => wt.Priority)
            .ThenBy(wt => wt.DueDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<WorkflowTask>> GetPendingTasksAsync()
    {
        return await _context.WorkflowTasks
            .Where(wt => !wt.IsDeleted && wt.Status == WorkflowTaskStatus.Pending)
            .Include(wt => wt.WorkflowInstance)
            .ThenInclude(wi => wi.Workflow)
            .Include(wt => wt.AssignedToUser)
            .Include(wt => wt.CreatedByUser)
            .OrderBy(wt => wt.Priority)
            .ThenBy(wt => wt.DueDate)
            .ToListAsync();
    }

    public async Task UpdateWorkflowInstanceStatusAsync(string instanceId, WorkflowInstanceStatus status)
    {
        var instance = await _context.WorkflowInstances
            .Where(wi => !wi.IsDeleted && wi.InstanceId == instanceId)
            .FirstOrDefaultAsync();

        if (instance != null)
        {
            instance.Status = status;
            instance.UpdatedAt = DateTime.UtcNow;

            if (status == WorkflowInstanceStatus.Completed)
            {
                instance.CompletedAt = DateTime.UtcNow;
            }
            else if (status == WorkflowInstanceStatus.Suspended)
            {
                instance.SuspendedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateTaskStatusAsync(Guid taskId, WorkflowTaskStatus status)
    {
        var task = await _context.WorkflowTasks
            .Where(wt => !wt.IsDeleted && wt.Id == taskId)
            .FirstOrDefaultAsync();

        if (task != null)
        {
            task.Status = status;
            task.UpdatedAt = DateTime.UtcNow;

            if (status == WorkflowTaskStatus.Completed)
            {
                task.CompletedAt = DateTime.UtcNow;
            }
            else if (status == WorkflowTaskStatus.InProgress)
            {
                task.StartedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }
    }

    public async Task<WorkflowTask?> GetTaskAsync(Guid taskId)
    {
        return await _context.WorkflowTasks
            .Where(wt => !wt.IsDeleted && wt.Id == taskId)
            .Include(wt => wt.WorkflowInstance)
            .ThenInclude(wi => wi.Workflow)
            .Include(wt => wt.AssignedToUser)
            .Include(wt => wt.CreatedByUser)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<WorkflowInstance>> GetWorkflowInstancesAsync(Guid workflowId)
    {
        return await _context.WorkflowInstances
            .Where(wi => !wi.IsDeleted && wi.WorkflowId == workflowId)
            .Include(wi => wi.Workflow)
            .Include(wi => wi.StartedByUser)
            .Include(wi => wi.AssignedToUser)
            .Include(wi => wi.Tasks)
            .OrderByDescending(wi => wi.CreatedAt)
            .ToListAsync();
    }
}

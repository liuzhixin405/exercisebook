using Microsoft.EntityFrameworkCore;
using AiDevPlatform.Core.Interfaces;
using AiDevPlatform.Shared.Models;
using AiDevPlatform.Shared.DTOs.Common;
using AiDevPlatform.Infrastructure.Data;

namespace AiDevPlatform.Infrastructure.Repositories;

public class DataModelRepository : Repository<DataModel>, IDataModelRepository
{
    public DataModelRepository(AiDevPlatformDbContext context) : base(context)
    {
    }

    public async Task<PagedResult<DataModel>> GetPagedDataModelsAsync(PagedRequest request, Guid applicationId)
    {
        IQueryable<DataModel> query = _dbSet
            .Where(dm => !dm.IsDeleted && dm.ApplicationId == applicationId)
            .Include(dm => dm.Application)
            .Include(dm => dm.FieldsList)
            .Include(dm => dm.RelationshipsList);

        // Apply search filter
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(dm => dm.Name.Contains(request.SearchTerm) ||
                                   dm.Description!.Contains(request.SearchTerm) ||
                                   dm.TableName.Contains(request.SearchTerm));
        }

        // Apply additional filters
        if (request.Filters != null)
        {
            foreach (var filter in request.Filters)
            {
                switch (filter.Key.ToLower())
                {
                    case "issystem":
                        if (bool.TryParse(filter.Value.ToString(), out var isSystem))
                        {
                            query = query.Where(dm => dm.IsSystem == isSystem);
                        }
                        break;
                    case "isreadonly":
                        if (bool.TryParse(filter.Value.ToString(), out var isReadOnly))
                        {
                            query = query.Where(dm => dm.IsReadOnly == isReadOnly);
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
                    query = request.SortDescending ? query.OrderByDescending(dm => dm.Name) : query.OrderBy(dm => dm.Name);
                    break;
                case "tablename":
                    query = request.SortDescending ? query.OrderByDescending(dm => dm.TableName) : query.OrderBy(dm => dm.TableName);
                    break;
                case "createdat":
                    query = request.SortDescending ? query.OrderByDescending(dm => dm.CreatedAt) : query.OrderBy(dm => dm.CreatedAt);
                    break;
                default:
                    query = query.OrderByDescending(dm => dm.CreatedAt);
                    break;
            }
        }
        else
        {
            query = query.OrderByDescending(dm => dm.CreatedAt);
        }

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return PagedResult<DataModel>.Create(items, totalCount, request.PageNumber, request.PageSize);
    }

    public async Task<DataModel?> GetDataModelWithFieldsAsync(Guid id)
    {
        return await _dbSet
            .Where(dm => !dm.IsDeleted && dm.Id == id)
            .Include(dm => dm.Application)
            .Include(dm => dm.FieldsList)
            .FirstOrDefaultAsync();
    }

    public async Task<DataModel?> GetDataModelWithRelationshipsAsync(Guid id)
    {
        return await _dbSet
            .Where(dm => !dm.IsDeleted && dm.Id == id)
            .Include(dm => dm.Application)
            .Include(dm => dm.RelationshipsList)
            .ThenInclude(r => r.ReferencedDataModel)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<DataModel>> GetApplicationDataModelsAsync(Guid applicationId)
    {
        return await _dbSet
            .Where(dm => !dm.IsDeleted && dm.ApplicationId == applicationId)
            .Include(dm => dm.Application)
            .Include(dm => dm.FieldsList)
            .OrderBy(dm => dm.Name)
            .ToListAsync();
    }

    public async Task<DataModel?> GetByTableNameAsync(string tableName, string? schema = null)
    {
        var query = _dbSet.Where(dm => !dm.IsDeleted && dm.TableName == tableName);

        if (!string.IsNullOrEmpty(schema))
        {
            query = query.Where(dm => dm.Schema == schema);
        }

        return await query
            .Include(dm => dm.Application)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> TableNameExistsAsync(string tableName, Guid applicationId, Guid? excludeId = null)
    {
        var query = _dbSet.Where(dm => !dm.IsDeleted && dm.TableName == tableName && dm.ApplicationId == applicationId);

        if (excludeId.HasValue)
        {
            query = query.Where(dm => dm.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<IEnumerable<DataModelField>> GetDataModelFieldsAsync(Guid dataModelId)
    {
        return await _context.DataModelFields
            .Where(f => !f.IsDeleted && f.DataModelId == dataModelId)
            .OrderBy(f => f.SortOrder)
            .ToListAsync();
    }

    public async Task<IEnumerable<DataModelRelationship>> GetDataModelRelationshipsAsync(Guid dataModelId)
    {
        return await _context.DataModelRelationships
            .Where(r => !r.IsDeleted && r.DataModelId == dataModelId)
            .Include(r => r.ReferencedDataModel)
            .ToListAsync();
    }

    public async Task<bool> ColumnNameExistsAsync(string columnName, Guid dataModelId, Guid? excludeId = null)
    {
        var query = _context.DataModelFields.Where(f => !f.IsDeleted && f.Name == columnName && f.DataModelId == dataModelId);

        if (excludeId.HasValue)
        {
            query = query.Where(f => f.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<DataModelField?> GetFieldByIdAsync(Guid fieldId)
    {
        return await _context.DataModelFields
            .Where(f => !f.IsDeleted && f.Id == fieldId)
            .Include(f => f.DataModel)
            .FirstOrDefaultAsync();
    }

    public async Task<DataModelRelationship?> GetRelationshipByIdAsync(Guid relationshipId)
    {
        return await _context.DataModelRelationships
            .Where(r => !r.IsDeleted && r.Id == relationshipId)
            .Include(r => r.DataModel)
            .Include(r => r.ReferencedDataModel)
            .FirstOrDefaultAsync();
    }

    public async Task AddFieldAsync(DataModelField field)
    {
        field.CreatedAt = DateTime.UtcNow;
        _context.DataModelFields.Add(field);
        await _context.SaveChangesAsync();
    }

    public async Task AddRelationshipAsync(DataModelRelationship relationship)
    {
        relationship.CreatedAt = DateTime.UtcNow;
        _context.DataModelRelationships.Add(relationship);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateFieldAsync(DataModelField field)
    {
        field.UpdatedAt = DateTime.UtcNow;
        _context.DataModelFields.Update(field);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRelationshipAsync(DataModelRelationship relationship)
    {
        relationship.UpdatedAt = DateTime.UtcNow;
        _context.DataModelRelationships.Update(relationship);
        await _context.SaveChangesAsync();
    }
}

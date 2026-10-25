using Microsoft.EntityFrameworkCore;
using AiDevPlatform.Core.Interfaces;
using AiDevPlatform.Shared.Models;
using AiDevPlatform.Infrastructure.Data;

namespace AiDevPlatform.Infrastructure.Repositories;

public class CodeTemplateRepository : Repository<CodeTemplate>, ICodeTemplateRepository
{
    public CodeTemplateRepository(AiDevPlatformDbContext context) : base(context)
    {
    }

    public async Task<bool> NameExistsAsync(string name, Guid? applicationId = null, Guid? excludeId = null)
    {
        var query = _dbSet.Where(ct => !ct.IsDeleted && ct.Name == name);

        if (applicationId.HasValue)
        {
            query = query.Where(ct => ct.ApplicationId == applicationId.Value);
        }

        if (excludeId.HasValue)
        {
            query = query.Where(ct => ct.Id != excludeId.Value);
        }

        return await query.AnyAsync();
    }

    public async Task<IEnumerable<CodeTemplate>> GetByTypeAsync(string templateType)
    {
        return await _dbSet
            .Where(ct => !ct.IsDeleted && ct.TemplateType == templateType && ct.IsActive)
            .OrderBy(ct => ct.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<CodeTemplate>> GetApplicationTemplatesAsync(Guid applicationId)
    {
        return await _dbSet
            .Where(ct => !ct.IsDeleted && ct.ApplicationId == applicationId && ct.IsActive)
            .OrderBy(ct => ct.TemplateType)
            .ThenBy(ct => ct.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<CodeTemplate>> GetSystemTemplatesAsync()
    {
        return await _dbSet
            .Where(ct => !ct.IsDeleted && ct.IsSystem && ct.IsActive)
            .OrderBy(ct => ct.TemplateType)
            .ThenBy(ct => ct.Name)
            .ToListAsync();
    }
}

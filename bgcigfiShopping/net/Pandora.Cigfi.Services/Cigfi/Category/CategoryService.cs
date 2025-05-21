using Pandora.Cigfi.IServices.Cigfi;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using System.Linq;
using FXH.Common.DapperService;
using Microsoft.Extensions.Logging;
using System;
using Pandora.Cigfi.Models.Cigfi;

namespace Pandora.Cigfi.Services.Cigfi
{
    public class CategoryService : BaseRepository<Models.Cigfi.Category>, ICategoryService
    {
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(IDapperRepository context, ILogger<CategoryService> logger) : base(context)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<CategoryViewModel>> GetAllAsync()
        {
            var sql = "SELECT * FROM cigfi_category ORDER BY SortOrder, Id";
            using (var connection = DbContext.Connection)
            {
                return await connection.QueryAsync<CategoryViewModel>(sql);
            }
        }

        public async Task<CategoryViewModel> GetByIdAsync(long id)
        {
            var sql = "SELECT * FROM cigfi_category WHERE Id = @Id";
            using (var connection = DbContext.Connection)
            {
                return (await connection.QueryAsync<CategoryViewModel>(sql, new { Id = id })).FirstOrDefault();
            }
        }

        public virtual async Task<bool> AddAsync(Models.Cigfi.Category category)
        {
            var sql = "INSERT INTO cigfi_category (Name, ParentId, SortOrder) VALUES (@Name, @ParentId, @SortOrder)";
            using (var connection = DbContext.Connection)
            {
                try
                {
                    var affectedRows = await connection.ExecuteAsync(sql, category);
                    if (affectedRows > 0)
                    {
                        _logger.LogInformation($"Category added successfully: {category.Name}");
                        return true;
                    }
                    _logger.LogWarning("No rows affected when adding category");
                    return false;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error adding category: {ex.Message}");
                    return false;
                }
            }
        }

        public virtual async Task<bool> UpdateAsync(Models.Cigfi.Category category)
        {
            var sql = "UPDATE cigfi_category SET Name = @Name, ParentId = @ParentId, SortOrder = @SortOrder WHERE Id = @Id";
            using (var connection = DbContext.Connection)
            {
                try
                {
                    var affectedRows = await connection.ExecuteAsync(sql, category);
                    if (affectedRows > 0)
                    {
                        _logger.LogInformation($"Category updated successfully: {category.Name}");
                        return true;
                    }
                    _logger.LogWarning("No rows affected when updating category");
                    return false;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error updating category: {ex.Message}");
                    return false;
                }
            }
        }

        public virtual async Task<bool> DeleteAsync(IEnumerable<long> ids)
        {
            var sql = "DELETE FROM cigfi_category WHERE Id IN @Ids";
            using (var connection = DbContext.Connection)
            {
                try
                {
                    var affectedRows = await connection.ExecuteAsync(sql, new { Ids = ids });
                    if (affectedRows > 0)
                    {
                        _logger.LogInformation($"Category deleted successfully: {string.Join(",", ids)}");
                        return true;
                    }
                    _logger.LogWarning("No rows affected when deleting category");
                    return false;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error deleting category: {ex.Message}");
                    return false;
                }
            }
        }
    }
}

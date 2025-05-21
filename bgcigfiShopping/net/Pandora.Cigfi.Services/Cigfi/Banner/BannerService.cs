using Pandora.Cigfi.Models.Cigfi;
using Pandora.Cigfi.IServices.Cigfi;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using System.Linq;
using System.Data;
using FXH.Common.DapperService;
using Microsoft.Extensions.Logging;
using System;

namespace Pandora.Cigfi.Services.Cigfi
{
    public class BannerService : BaseRepository<Banner>, IBannerService
    {
        private readonly ILogger<BannerService> _logger;

        public BannerService(IDapperRepository context, ILogger<BannerService> logger) : base(context)
        {
            _logger = logger;
        }

        public override async Task<IEnumerable<Banner>> GetAllAsync()
        {
            var sql = "SELECT * FROM cigfi_banner ORDER BY SortOrder, Id";
            using (var connection = DbContext.Connection)
            {
                return await connection.QueryAsync<Banner>(sql);
            }
        }

        public async Task<Banner> GetByIdAsync(long id)
        {
            var sql = "SELECT * FROM cigfi_banner WHERE Id = @Id";
            using (var connection = DbContext.Connection)
            {
                return (await connection.QueryAsync<Banner>(sql, new { Id = id })).FirstOrDefault();
            }
        }

        public async Task AddAsync(Banner banner)
        {
            var sql = "INSERT INTO cigfi_banner (ImageUrl, LinkUrl, SortOrder, IsActive, CreatedAt, UpdatedAt) VALUES (@ImageUrl, @LinkUrl, @SortOrder, @IsActive, @CreatedAt, @UpdatedAt)";
            using (var connection = DbContext.Connection)
            {
                try
                {
                    await connection.ExecuteAsync(sql, banner);
                    _logger.LogInformation($"Banner added successfully: {banner.ImageUrl}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error adding banner: {ex.Message}");
                    throw;
                }
            }
        }

        public override async Task<bool> UpdateAsync(Banner banner)
        {
            var sql = "UPDATE cigfi_banner SET ImageUrl = @ImageUrl, LinkUrl = @LinkUrl, SortOrder = @SortOrder, IsActive = @IsActive, UpdatedAt = @UpdatedAt WHERE Id = @Id";
            using (var connection = DbContext.Connection)
            {
                try
                {
                    var rowsAffected = await connection.ExecuteAsync(sql, banner);
                    _logger.LogInformation($"Banner updated successfully: {banner.Id}");
                    return rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error updating banner: {ex.Message}");
                    throw;
                }
            }
        }

        public async Task DeleteAsync(IEnumerable<long> ids)
        {
            var sql = "DELETE FROM cigfi_banner WHERE Id IN @Ids";
            using (var connection = DbContext.Connection)
            {
                try
                {
                    await connection.ExecuteAsync(sql, new { Ids = ids });
                    _logger.LogInformation($"Banner(s) deleted successfully: {string.Join(",", ids)}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error deleting banner(s): {ex.Message}");
                    throw;
                }
            }
        }

        Task IBannerService.UpdateAsync(Banner banner)
        {
            return UpdateAsync(banner);
        }
    }
}
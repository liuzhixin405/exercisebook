using Dapper;
using FXH.Common.DapperService;
using Microsoft.Extensions.Logging;
using Pandora.Cigfi.IServices.Cigfi;
using Pandora.Cigfi.Models.Cigfi.Invitation;
using Pandora.Cigfi.Models.Cigfi.Product;
using Pandora.Cigfi.Models.Cigfi.Rebate;
using Pandora.Cigfi.Models.ResponseMsg;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandora.Cigfi.Services.Cigfi
{
    public class RebateService : BaseRepository<Rebate>, IRebateService
    {
        private readonly ILogger<Rebate> _logger;
        public RebateService(IDapperRepository context, ILogger<Rebate> logger) : base(context)
        {
            _logger = logger;
        }

        // 分页查询
        public async Task<PagedResult<RebateViewModel>> GetPageListAsync(Hashtable queryHt, int page = 1, int limit = 20)
        {
            string sql = "SELECT * FROM cigfi_rebate ";
            var parameters = new DynamicParameters();
            sql += GetWhere(queryHt, parameters);
            sql += $" ORDER BY level ASC LIMIT {(page - 1) * limit},{limit}";
            using ( var connection = DbContext.Connection)
            {
                var list = await connection.QueryAsync<RebateViewModel>(sql, parameters);
                return new PagedResult<RebateViewModel>
                {
                    Total = await CountAsync(queryHt),
                    Items = list.ToList()
                };
            }
        }

        // 总数
        public async Task<int> CountAsync(Hashtable queryHt)
        {
            string sql = "SELECT COUNT(*) FROM cigfi_rebate ";
            var parameters = new DynamicParameters();
            sql += GetWhere(queryHt, parameters);
            using (var connection = DbContext.Connection)
            {
                return await connection.ExecuteScalarAsync<int>(sql, parameters);
            }
        }

        // 根据ID获取详情
        public async Task<RebateViewModel> GetByIdAsync(int id)
        {
            string sql = "SELECT * FROM cigfi_rebate WHERE id = @Id";
            using (var connection = DbContext.Connection)
            {
                return await connection.QueryFirstOrDefaultAsync<RebateViewModel>(sql, new { Id = id });
            }
        }

        // 添加
        public async Task<bool> AddAsync(Rebate model)
        {
            string sql = @"INSERT INTO cigfi_rebate(level, InviterLevelDesc, RebateRatio)
                       VALUES (@Level, @InviterLevelDesc, @RebateRatio)";
            try
            {
                using (var connection = DbContext.Connection)
                {
                    var rows = await connection.ExecuteAsync(sql, model);
                    if (rows > 0)
                    {
                        _logger.LogInformation($"新增返佣配置成功，等级：{model.Level}");
                        return true;
                    }
                    _logger.LogWarning("新增返佣配置无数据变化");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"新增返佣配置异常：{ex.Message}");
                return false;
            }
        }

        // 修改
        public async Task<bool> UpdateAsync(Rebate model)
        {
            string sql = @"UPDATE cigfi_rebate
                       SET level = @Level,
                           InviterLevelDesc = @InviterLevelDesc,
                           RebateRatio = @RebateRatio
                       WHERE id = @Id";
            using (var connection = DbContext.Connection)
            {
                var rows = await connection.ExecuteAsync(sql, model);
                return rows > 0;
            }
        }

        // 删除
        public async Task<bool> DeleteAsync(List<int> ids)
        {
            string sql = "DELETE FROM cigfi_rebate WHERE id IN @Ids";
            using (var connection = DbContext.Connection)
            {
                var rows = await connection.ExecuteAsync(sql, new { Ids = ids });
                return rows > 0;
            }
        }

        // 构造条件语句（目前只支持关键词模糊匹配，比如描述）
        private string GetWhere(Hashtable queryHt, DynamicParameters parameters)
        {
            string where = "";
            if (queryHt.Count > 0)
            {
                if (queryHt.Contains("keyword") && !string.IsNullOrEmpty(queryHt["keyword"]?.ToString()))
                {
                    where += " InviterLevelDesc LIKE @Keyword ";
                    parameters.Add("Keyword", $"%{queryHt["keyword"]}%");
                }
            }
            if (!string.IsNullOrEmpty(where))
            {
                where = " WHERE " + where;
            }
            return where;
        }
    }
}


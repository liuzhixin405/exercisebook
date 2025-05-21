using Dapper;
using FXH.Common.DapperService;
using Microsoft.Extensions.Logging;
using Pandora.Cigfi.IServices.Cigfi;
using Pandora.Cigfi.Models.Cigfi.Invitation;
using Pandora.Cigfi.Models.ResponseMsg;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pandora.Cigfi.Services.Cigfi.Invitation
{
    public class InvitationService : BaseRepository<CigfiMember>, IInvitationService
    {
        private readonly ILogger<CigfiMember> _logger;
        public InvitationService(IDapperRepository context, ILogger<CigfiMember> logger) : base(context)
        {
            _logger = logger;
        }

        public async Task<PagedResult<CigfiMemberViewModel>> GetPageListAsync(Hashtable queryHt, int page = 1, int limit = 20)
        {
            string sql = "SELECT * FROM cigfi_member";
            var parameters = new DynamicParameters();
            sql += GetWhere(queryHt, parameters);
            sql += $" ORDER BY Id DESC LIMIT {(page - 1) * limit},{limit}";
            using (var connection = DbContext.Connection)
            {
                var list = await connection.QueryAsync<CigfiMemberViewModel>(sql, parameters);
                return new PagedResult<CigfiMemberViewModel>
                {
                    Total = await CountAsync(queryHt),
                    Items = list.ToList()
                };
            }
        }

        public async Task<int> CountAsync(Hashtable queryHt)
        {
            string sql = "SELECT COUNT(*) FROM cigfi_member";
            var parameters = new DynamicParameters();
            sql += GetWhere(queryHt, parameters);
            using (var connection = DbContext.Connection)
            {
                return await connection.ExecuteScalarAsync<int>(sql, parameters);
            }
        }

        public async Task<CigfiMemberViewModel> GetByIdAsync(long id)
        {
            string sql = "SELECT * FROM cigfi_member WHERE Id=@Id";
            using (var connection = DbContext.Connection)
            {
                return await connection.QueryFirstOrDefaultAsync<CigfiMemberViewModel>(sql, new { Id = id });
            }
        }

        public async Task<bool> AddAsync(CigfiMember model)
        {
            string checkSql = "SELECT COUNT(1) FROM cigfi_member WHERE UserId=@UserId AND WalletAddress=@WalletAddress";
            using (var connection = DbContext.Connection)
            {
                var exists = await connection.ExecuteScalarAsync<int>(checkSql, new { model.UserId, model.WalletAddress });
                if (exists > 0)
                {
                    _logger.LogWarning($"Duplicate CigfiMember: {model.UserId} in WalletAddress {model.WalletAddress}");
                    return false;
                }

                string sql = @"INSERT INTO cigfi_member 
        (UserId, WalletAddress, IsVip, InviteCode, InvitedBy, RebateAmount, CreatedAt, UpdatedAt)
        VALUES (@UserId, @WalletAddress, @IsVip, @InviteCode, @InvitedBy, @RebateAmount, @CreatedAt, @UpdatedAt)";

                try
                {
                    var affectedRows = await connection.ExecuteAsync(sql, model);
                    if (affectedRows > 0)
                    {
                        _logger.LogInformation($"CigfiMember added successfully: {model.UserId}");
                        return true;
                    }
                    _logger.LogWarning("No rows affected when adding CigfiMember");
                    return false;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error adding CigfiMember: {ex.Message}");
                    return false;
                }
            }
        }

        public async Task<bool> UpdateAsync(CigfiMember model)
        {
            using (var connection = DbContext.Connection)
            {
                string sql = @"UPDATE cigfi_member SET 
        UserId=@UserId, WalletAddress=@WalletAddress, IsVip=@IsVip, 
        InviteCode=@InviteCode, InvitedBy=@InvitedBy, RebateAmount=@RebateAmount, 
        UpdatedAt=@UpdatedAt WHERE Id=@Id";

                return await connection.ExecuteAsync(sql, model) > 0;
            }
        }

        public async Task<bool> DeleteAsync(List<long> ids)
        {
            string sql = "DELETE FROM cigfi_member WHERE Id IN @Ids";
            using (var connection = DbContext.Connection)
            {
                return await connection.ExecuteAsync(sql, new { Ids = ids }) > 0;
            }
        }

        private string GetWhere(Hashtable queryHt, DynamicParameters parameters)
        {
            var where = new List<string>();
            if (queryHt.Contains("keyword"))
            {
                where.Add("(WalletAddress LIKE @Keyword OR InviteCode LIKE @Keyword)");
                parameters.Add("Keyword", $"%{queryHt["keyword"]}%");
            }

            return where.Count > 0 ? " WHERE " + string.Join(" AND ", where) : string.Empty;
        }
    }
}

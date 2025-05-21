using Dapper;
using Pandora.Cigfi.IServices.Cigfi;
using Pandora.Cigfi.Models.Cigfi.Order;
using Pandora.Cigfi.Models.ResponseMsg;
using FXH.Common.DapperService;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pandora.Cigfi.Services.Cigfi
{
    public class OrderService : BaseRepository<Order>, IOrderService
    {
        public OrderService(IDapperRepository context) : base(context) { }

        public async Task<PagedResult<OrderViewModel>> GetPageListAsync(Hashtable queryHt, int page = 1, int limit = 20)
        {
            string sql = "SELECT * FROM cigfi_order ";
            var parameters = new DynamicParameters();
            sql += GetWhere(queryHt, parameters);
            sql += $" ORDER BY Id DESC LIMIT {(page - 1) * limit},{limit}";
            using (var connection = DbContext.Connection)
            {
                var list = await connection.QueryAsync<OrderViewModel>(sql, parameters);
                return new PagedResult<OrderViewModel>
                {
                    Total = await CountAsync(queryHt),
                    Items = list.ToList()
                };
            }
        }

        public async Task<int> CountAsync(Hashtable queryHt)
        {
            string sql = "SELECT COUNT(*) FROM cigfi_order ";
            var parameters = new DynamicParameters();
            sql += GetWhere(queryHt, parameters);
            using (var connection = DbContext.Connection)
            {
                return await connection.ExecuteScalarAsync<int>(sql, parameters);
            }
        }

        public async Task<OrderViewModel> GetByIdAsync(int id)
        {
            string sql = "SELECT * FROM cigfi_order WHERE Id=@Id";
            using (var connection = DbContext.Connection)
            {
                return await connection.QueryFirstOrDefaultAsync<OrderViewModel>(sql, new { Id = id });
            }
        }

        public async Task<bool> UpdateAsync(OrderViewModel model)
        {
            string sql = "UPDATE cigfi_order SET Status=@Status, PayStatus=@PayStatus, Remark=@Remark WHERE Id=@Id";
            using (var connection = DbContext.Connection)
            {
                return await connection.ExecuteAsync(sql, model) > 0;
            }
        }

        public async Task<bool> DeleteAsync(List<int> ids)
        {
            string sql = "DELETE FROM cigfi_order WHERE Id IN @Ids";
            using (var connection = DbContext.Connection)
            {
                return await connection.ExecuteAsync(sql, new { Ids = ids }) > 0;
            }
        }

        private string GetWhere(Hashtable queryHt, DynamicParameters parameters)
        {
            string where = string.Empty;
            if (queryHt.Count > 0)
            {
                if (queryHt.Contains("orderno"))
                {
                    where += " OrderNo LIKE @OrderNo ";
                    parameters.Add("OrderNo", $"%{queryHt["orderno"]}%");
                }
                if (queryHt.Contains("userid"))
                {
                    if (!string.IsNullOrEmpty(where)) where += " AND ";
                    where += " UserId = @UserId ";
                    parameters.Add("UserId", queryHt["userid"]);
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

using Dapper;
using Pandora.Cigfi.IServices.Sys;
using Pandora.Cigfi.Models.Consts;
using Pandora.Cigfi.Models.Sys;
using FXH.Common.DapperService;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pandora.Cigfi.Services.Sys
{
    public class OperationLogViewService : BaseRepository<Sys_ReviewLogViewModel>, IOperationLogViewService
    {
        public OperationLogViewService(IDapperRepository context) : base(context)
        {


        }

        public async Task<int> CountAsync(Hashtable queryHt)
        {
            string sql = $"select count(*) from  {TableNameConst.SYS_REVIEW_LOG} r left join {TableNameConst.SYS_ADMIN} a on  a.`Id`=r.`Operator` ";
            var parameters = new DynamicParameters();
            sql += GetWhere(queryHt, parameters);
            using (var connection = DbContext.Connection)
            {
                return await connection.ExecuteScalarAsync<int>(sql, parameters);
            }
        }

        public async Task<Sys_ReviewLogViewModel> GetModelById(int Id)
        {
            string sql = string.Format("select r.*,a.RealName OperationPeople  from  {0} r left join {1}  a on  a.Id=r.Operator  where r.ID=@ID ", TableNameConst.SYS_REVIEW_LOG, TableNameConst.SYS_ADMIN);
            using (var connection = DbContext.Connection)
            {
                return await connection.QueryFirstOrDefaultAsync<Sys_ReviewLogViewModel>(sql, new { ID = Id });

            }
        }

        public async Task<IEnumerable<Sys_ReviewLogViewModel>> GetPageListAsync(Hashtable queryHt, int page = 1, int limit = 20)
        {
            string sql = string.Format("select r.*,a.RealName OperationPeople  from  {0} r left join {1}  a on  a.Id=r.Operator  ", TableNameConst.SYS_REVIEW_LOG, TableNameConst.SYS_ADMIN);
            var parameters = new DynamicParameters();
            sql += GetWhere(queryHt, parameters);
            sql += string.Format("  limit {0} ,{1}", (page - 1) * limit, limit);

            using (var connection = DbContext.Connection)
            {
                return await connection.QueryAsync<Sys_ReviewLogViewModel>(sql, parameters);

            }
        }

        private string GetWhere(Hashtable queryHt, DynamicParameters parameters)
        {
            var where = "WHERE 1 = 1 ";
            if (queryHt.Count > 0)
            {
                string operators = " LIKE ";
                string keyStart = "%";
                string keyEnd = "%";
                object isAccurate = queryHt["isAccurate"];
                if (isAccurate != null && isAccurate.ToString() == "true")
                {
                    operators = " = ";
                    keyStart = "";
                    keyEnd = "";
                }


                var operationCode = queryHt["operationCode"];
                if (operationCode != null)
                {
                    where += " AND r.Code " + operators + " @operationCode ";
                    parameters.Add("operationCode", $"{keyStart + operationCode + keyEnd}");
                }

                var operationObject = queryHt["operationObject"];
                if (operationObject != null)
                {

                    where += " AND r.TargetType=" + Convert.ToInt32(operationObject);
                }



                var operationPeople = queryHt["operationPeople"];
                if (operationPeople != null)
                {

                    where += " AND r.Operator=" + Convert.ToInt32(operationPeople);
                }
                var operationType = queryHt["operationType"];
                if (operationType != null)
                {

                    where += " AND r.OperateType=" + Convert.ToInt32(operationType);
                }


                var sortKey = queryHt["sortKey"];
                if (sortKey != null)
                {
                    switch (sortKey)
                    {
                        case "OpTime":
                            where += " order by r.OpTime ";
                            break;
                        default:
                            where += " order by r.OpTime ";
                            break;
                    }
                    var sort = queryHt["sort"];
                    if (sort != null)
                    {
                        var sortStirng = sort.ToString();
                        if (sortStirng == "DESC")
                        {
                            where += " DESC ";
                        }
                        if (sortStirng == "ASC")
                        {
                            where += " ASC ";
                        }
                    }
                }
            }
            return where;
        }
    }
}

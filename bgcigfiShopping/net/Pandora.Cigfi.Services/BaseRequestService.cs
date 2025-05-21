using Dapper;
using Dapper.Contrib.Extensions;
using Pandora.Cigfi.Common.Requests;
using Pandora.Cigfi.IServices;
using Pandora.Cigfi.Models.Consts;
using FXH.Common.DapperService;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pandora.Cigfi.Services
{
    public abstract class BaseRequestService<T> : BaseRepository<T>, IBaseService_NoPaging<T> where T : class, new()
    {
        protected BaseRequestService(IDapperRepository context) : base(context)
        {
        }
        public virtual async Task<T> FindEntity(QitemCollection q)
        {
            StringBuilder sql = new StringBuilder($"select * from  {GetTableName<T>()} a where 1=1 ");
            sql.Append(string.Join(' ', q));
            using (var conn = base.DbContext.GetConnection())
            {
                return await conn.QueryFirstOrDefaultAsync<T>(sql.ToString(), q.ToParam());
            }
        }
        /// <summary>
        /// 分页读取数据
        /// </summary>
        /// <param name="queryHt">过滤条件</param>
        /// <param name="page">页码</param>
        /// <param name="limit">limit数量</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<T>> GetPageListAsync(AdminCPRequest fxhRequest)
        {
            StringBuilder sql = new StringBuilder($"select * from  {GetTableName<T>()} a where 1=1 ");
            BuilderSql(fxhRequest, sql);
            using (var conn = base.DbContext.GetConnection())
            {
                return await conn.QueryAsync<T>(sql.ToString(), fxhRequest.q.ToParam());
            }
        }
        /// <summary>
        /// 根据过滤条件统计符合条件的记录数
        /// </summary>
        /// <param name="queryHt"></param>
        /// <returns></returns>
        public virtual async Task<int> CountAsync(AdminCPRequest fxhRequest)
        {
            StringBuilder sql = new StringBuilder($"select count(1) from  {GetTableName<T>()} a where 1=1 ");
            BuilderWhereSql(fxhRequest, sql);
            using (var conn = base.DbContext.GetConnection())
            {
                return await conn.ExecuteScalarAsync<int>(sql.ToString(), fxhRequest.q.ToParam());
            }
        }
        /// <summary>
        /// 获取表名
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        protected string GetTableName<T>()
        {
            var tabletype = typeof(TableAttribute);
            var tableAttribute = (TableAttribute)Attribute.GetCustomAttribute(typeof(T), tabletype);
            return tableAttribute.Name;
        }

        /// <summary>
        /// 追加where ，分页，排序
        /// </summary>
        public virtual void BuilderSql(AdminCPRequest fxhRequest, StringBuilder sql)
        {
            if (fxhRequest != null)
            {
                BuilderWhereSql(fxhRequest, sql);
                if (fxhRequest.Sort != null && fxhRequest.Sort.Count > 0)
                {
                    sql.Append(" order by ");
                    foreach (var sortitem in fxhRequest.Sort)
                    {
                        if (!string.IsNullOrEmpty(sortitem.Name))
                        {
                            sql.Append($" {sortitem.Name} {(sortitem.Order == "asc" ? "asc" : "desc")},");
                        }
                    }
                    sql.Length--;
                }
                sql.Append($" limit {((fxhRequest.Page - 1) * fxhRequest.Limit)},{fxhRequest.Limit}");
            }
        }
        /// <summary>
        /// 追加where 
        /// </summary>
        public virtual void BuilderWhereSql(AdminCPRequest fxhRequest, StringBuilder sql)
        {
            if (fxhRequest != null)
                sql.Append(string.Join(' ', fxhRequest.q));
        }
    }
}

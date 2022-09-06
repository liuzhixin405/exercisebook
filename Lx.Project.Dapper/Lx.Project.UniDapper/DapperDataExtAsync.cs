using Dapper;
using Dapper.Contrib.Extensions;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Lx.Project.UniDapper
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    public abstract partial class DapperDataAsync
    {
        #region 查询系
        /// <summary>
        /// 获取Model-Key为int类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static async Task<T> GetAsync<T>(MySqlConnection connection, int id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return await connection.GetAsync<T>(id, transaction, commandTimeout);
        }
        /// <summary>
        /// 获取Model-Key为long类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static async Task<T> GetAsync<T>(MySqlConnection connection, long id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return await connection.GetAsync<T>(id, transaction, commandTimeout);
        }
        /// <summary>
        /// 获取Model-Key为Guid类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static async Task<T> GetAsync<T>(MySqlConnection connection, System.Guid id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return await connection.GetAsync<T>(id, transaction, commandTimeout);
        }
        /// <summary>
        /// 获取Model-Key为string类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static async Task<T> GetAsync<T>(MySqlConnection connection, string id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return await connection.GetAsync<T>(id, transaction, commandTimeout);
        }
        /// <summary>
        /// 获取Model集合（没有Where条件）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<IEnumerable<T>> GetAllAsync<T>(MySqlConnection connection) where T : class
        {
            return await connection.GetAllAsync<T>();
        }
        #endregion

        #region 增删改
        /// <summary>
        /// 插入一个Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="sqlAdapter"></param>
        /// <returns></returns>
        public static async Task<int> InsertAsync<T>(MySqlConnection connection, T model, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return await connection.InsertAsync<T>(model, transaction, commandTimeout);
        }
        /// <summary>
        /// 删除一个Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="sqlAdapter"></param>
        /// <returns></returns>
        public static async Task<bool> DeleteAsync<T>(MySqlConnection connection, T model, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return await connection.DeleteAsync<T>(model, transaction, commandTimeout);
        }
        /// <summary>
        /// 更新一个Model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <param name="entityToUpdate"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public static async Task<bool> UpdateAsync<T>(MySqlConnection connection, T model, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            return await connection.UpdateAsync<T>(model, transaction, commandTimeout);
        }
        #endregion

        #region 分页查询
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="sql">查询语句</param>
        /// <param name="p">动态参数</param>
        /// <param name="sqlTotal">total语句</param>
        /// <param name="p2">Total动态参数</param>
        /// <returns></returns>
        public static async Task<string> PageLoadAsync<T>(MySqlConnection connection, string sql, object p = null, string sqlTotal = "", object p2 = null)
        {
            var rows = await QueryAsync(connection, sql.ToString(), p);
            var total = rows.Count();
            if (!string.IsNullOrWhiteSpace(sqlTotal)) { total = await ExecuteScalarAsync<int>(connection, sqlTotal, p2); }
            return JsonConvert.SerializeObject(new { rows = rows, total = total });
        }

        public static async Task<Tuple<int, IEnumerable<T>>> PageLoadAsync<T>(MySqlConnection connection, string sql, object param = null, int pageIndex = 1, int pageSize = 20, string sortBySql = "")
        {
            int total = await ExecuteScalarAsync<int>(connection, $"select count(*) {sql.Substring(sql.ToLower().IndexOf("from"))}", param);

            if (!string.IsNullOrEmpty(sortBySql))
                sql += $" {sortBySql} ";

            sql += $" LIMIT {(pageIndex - 1) * pageSize},{pageSize}";
            var query = await QueryAsync<T>(connection, sql, param);
            return Tuple.Create(total, query);
        }

        public static async Task<DbTransaction> BeginTransaction(MySqlConnection connection)
        {
            return await connection.BeginTransactionAsync();
        }

        public static async Task<Tuple<int, IEnumerable<T>, IEnumerable<E>, IEnumerable<Q>>> QueryMultipleAsync<T, E, Q>(MySqlConnection connection, StringBuilder sql, object param = null)
        {
            int total = 0;
            IEnumerable<T> tlist;
            IEnumerable<E> elist;
            IEnumerable<Q> qlist;
            using (var multi = await connection.QueryMultipleAsync(sql.ToString(), param))
            {
                total = multi.Read<int>().ToList().Count;
                tlist = multi.Read<T>().ToList();
                elist = multi.Read<E>();
                qlist = multi.Read<Q>();
            }
            return Tuple.Create(total, tlist, elist, qlist);
        }
        #endregion
    }
}

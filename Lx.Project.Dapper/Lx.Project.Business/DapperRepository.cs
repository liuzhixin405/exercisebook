using Lx.Project.UniDapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Lx.Project.Business
{
    /// <summary>
    /// 连接字符串工厂类
    /// </summary>
    public partial class ConnFactory
    {
        private static readonly string connString ="获取配置文件的连接字符串";
        /// <summary>
        /// 获取Connection
        /// </summary>
        /// <returns></returns>
        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connString);
        }
    }

    public partial class DapperRepository
    {

        #region 初始化字段属性    
        #endregion
        #region 基础方法


        /// <summary>
        /// sql操作 返回受影响的ID
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public async Task<T> ExecuteScalarAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            if (transaction != null)
            {
                return await DapperDataAsync.ExecuteScalarAsync<T>((MySqlConnection)transaction.Connection, sql, param, transaction, commandTimeout, commandType);
            }
            using (var conn = ConnFactory.GetConnection())
            {
                await conn.OpenAsync();
                return await DapperDataAsync.ExecuteScalarAsync<T>(conn, sql, param, transaction, commandTimeout, commandType);
            }
        }
        #region 查询系列
        /// <summary>
        /// 强类型查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="buffered"></param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            using (var conn = ConnFactory.GetConnection())
            {
                await conn.OpenAsync();
                return await DapperDataAsync.QueryAsync<T>(conn, sql, param, transaction, commandTimeout, commandType);
            }

        }


        public async Task<T> QuerySingleAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            using (var conn = ConnFactory.GetConnection())
            {
                await conn.OpenAsync();
                return await DapperDataAsync.QuerySingleAsync<T>(conn, sql, param, transaction, commandTimeout, commandType);
            }
        }

        /// <summary>
        /// 动态类型查询 | 多映射动态查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="buffered"></param>
        /// <returns></returns>
        public async Task<IEnumerable<dynamic>> QueryAsyncDynamic(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            using (var conn = ConnFactory.GetConnection())
            {
                return await DapperDataAsync.QueryAsync(conn, sql, param, transaction, commandTimeout, commandType);
            }
        }
        #endregion

        #region sql操作返回受影响行数
        /// <summary>
        /// 返回受影响的行数 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public async Task<int> ExecuteAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = default(int?), CommandType? commandType = default(CommandType?))
        {
            if (transaction != null)
            {
                return await DapperDataAsync.ExecuteAsync((MySqlConnection)transaction.Connection, sql, param, transaction, commandTimeout, commandType);
            }
            using (var conn = ConnFactory.GetConnection())
            {
                await conn.OpenAsync();
                return await DapperDataAsync.ExecuteAsync(conn, sql, param, transaction, commandTimeout, commandType);
            }
        }
        #endregion

        #endregion

        #region 扩展方法

        #region 查询系
        /// <summary>
        /// 获取Model-Key为int类型
        /// </summary>
        /// <param name="id"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(int id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
        {
            using (var conn = ConnFactory.GetConnection())
            {
                return await DapperDataAsync.GetAsync<T>(conn, id, transaction, commandTimeout);
            }
        }
        /// <summary>
        /// 获取Model-Key为long类型
        /// </summary>
        /// <param name="id"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(long id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
        {
            using (var conn = ConnFactory.GetConnection())
            {
                await conn.OpenAsync();
                return await DapperDataAsync.GetAsync<T>(conn, id, transaction, commandTimeout);
            }
        }
        /// <summary>
        /// 获取Model-Key为Guid类型
        /// </summary>
        /// <param name="id"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(System.Guid id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
        {
            using (var conn = ConnFactory.GetConnection())
            {
                await conn.OpenAsync();
                return await DapperDataAsync.GetAsync<T>(conn, id, transaction, commandTimeout);
            }
        }
        /// <summary>
        /// 获取Model-Key为String类型
        /// </summary>
        /// <param name="id"></param>
        /// <param name="transaction"></param>
        /// <param name="commandTimeout"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class, new()
        {
            using (var conn = ConnFactory.GetConnection())
            {
                await conn.OpenAsync();
                return await DapperDataAsync.GetAsync<T>(conn, id, transaction, commandTimeout);
            }
        }
        /// <summary>
        /// 获取Model集合（没有Where条件）
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<T>> GetAllAsync<T>() where T : class, new()
        {
            using (var conn = ConnFactory.GetConnection())
            {
                return await DapperDataAsync.GetAllAsync<T>(conn);
            }

        }
        #endregion

        #region 增删改

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
        public async Task<string> PageLoadAsync<T>(string sql, object p = null, string sqlTotal = "", object p2 = null)
        {
            using (var conn = ConnFactory.GetConnection())
            {
                return await DapperDataAsync.PageLoadAsync<T>(conn, sql, p, sqlTotal, p2);
            }
        }

        public async Task<Tuple<int, IEnumerable<T>>> PageLoadAsync<T>(string sql, object param = null, int pageIndex = 1, int pageSize = 20, string sortBySql = "")
        {
            using (var conn = ConnFactory.GetConnection())
            {
                return await DapperDataAsync.PageLoadAsync<T>(conn, sql, param, pageIndex, pageSize, sortBySql);
            }
        }

        public async Task<Tuple<int, IEnumerable<T>, IEnumerable<E>, IEnumerable<Q>>> PageListAsync<T, E, Q>(StringBuilder sql, object param = null)
        {
            using (var conn = ConnFactory.GetConnection())
            {
                return await DapperDataAsync.QueryMultipleAsync<T, E, Q>(conn, sql, param);
            }
        }

        #endregion


        /// <summary>
        /// Inserts an entity into table "Ts" asynchronously using Task and returns identity id.
        /// </summary>
        /// <typeparam name="T">The type being inserted.</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <returns>Identity of inserted entity</returns>
        public async Task<int> InsertAsync<TEntity>(TEntity entity, IDbTransaction trans = null, int? commandTimeout = null) where TEntity : class, new()
        {
            if (trans != null)
            {
                return await DapperDataAsync.InsertAsync<TEntity>((MySqlConnection)trans.Connection, entity, trans, commandTimeout);
            }
            using (var conn = ConnFactory.GetConnection())
            {
                return await DapperDataAsync.InsertAsync<TEntity>(conn, entity, trans, commandTimeout);
            }

        }

        /// <summary>
        /// Updates entity in table "Ts" asynchronously using Task, checks if the entity is modified if the entity is tracked by the Get() extension.
        /// </summary>
        /// <typeparam name="T">Type to be updated</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <returns>true if updated, false if not found or not modified (tracked entities)</returns>
        public async Task<bool> UpdateAsync<TEntity>(TEntity entity, IDbTransaction trans = null, int? commandTimeout = null) where TEntity : class, new()
        {
            if (trans != null)
            {
                return await DapperDataAsync.UpdateAsync<TEntity>((MySqlConnection)trans.Connection, entity, trans, commandTimeout);
            }
            using (var conn = ConnFactory.GetConnection())
            {
                return await DapperDataAsync.UpdateAsync<TEntity>(conn, entity, trans, commandTimeout);
            }
        }

        /// <summary>
        /// Delete entity in table "Ts" asynchronously using Task.
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <returns>true if deleted, false if not found</returns>
        public async Task<bool> DeleteAsync<TEntity>(TEntity entity, IDbTransaction trans = null, int? commandTimeout = null) where TEntity : class
        {
            if (trans != null)
            {
                return await DapperDataAsync.DeleteAsync<TEntity>((MySqlConnection)trans.Connection, entity, trans, commandTimeout);
            }
            using (var conn = ConnFactory.GetConnection())
            {
                await conn.OpenAsync();
                return await DapperDataAsync.DeleteAsync<TEntity>(conn, entity, trans, commandTimeout);
            }
        }

        /// <summary>
        /// 事务开启 单独调用记得释放
        /// </summary>
        /// <returns></returns>
        public async Task<IDbTransaction> BeginTransaction()
        {
            var conn = ConnFactory.GetConnection();
            await conn.OpenAsync();
            return await DapperDataAsync.BeginTransaction(conn);
        }

        /// <summary>
        /// 事务封装
        /// </summary>
        /// <typeparam name="TL">接收返回值</typeparam>
        /// <param name="func"></param>
        /// <returns></returns>待执行事件
        public async Task<TL> BeginTransaction<TL>(Func<IDbTransaction, TL> func)
        {
            TL res = default;
            using (IDbTransaction conn = await this.BeginTransaction())
            {
                try
                {
                    if (func != null)
                    {
                        res = func.Invoke(conn);
                    }
                    var t = (res as Task);
                    t?.Wait();
                    conn.Commit();
                }
                catch (Exception ex)
                {
                    conn.Rollback();
                    throw ex;
                }
                finally
                {
                    conn.Connection.Close();
                }
                return res;
            }
        }
        #endregion
    }
}

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mfc.Core
{
    public class MySqlDatabaseProvider:IDatabaseProvider
    {
        private readonly string _connectionString;

        public MySqlDatabaseProvider(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> ExecuteNonQueryAsync(string query)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand(query, connection))
                {
                  return  await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<List<T>> ExecuteQueryAsync<T>(string query)
        {
            var result = new List<T>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new MySqlCommand(query, connection))
                {
                    var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var entity = Activator.CreateInstance<T>();
                        foreach (var property in typeof(T).GetProperties())
                        {
                            property.SetValue(entity, reader[property.Name]);
                        }
                        result.Add(entity);
                    }
                }
            }
            return result;
        }

        public async Task<int> ExecuteScalarAsync(string sql)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new MySqlCommand(sql, connection))
                    {
                        var result = await command.ExecuteScalarAsync();
                        if (result != null && int.TryParse(result.ToString(), out var value))
                        {
                            return value; // 返回查询结果
                        }
                        return 0; // 如果查询结果为空或无法转换为 int
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing scalar query: {ex.Message}");
                throw;
            }
        }

        public Task<TEntity> FindAsync<TEntity>(Func<TEntity, bool> predicate) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public async Task<List<TEntity>> Query<TEntity>(string selectClause = "*",    string whereClause = null,    object parameters = null,    string orderBy = null,    int? skip = null,    int? take = null) where TEntity : class
        {
            var entities = new List<TEntity>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync(); // 异步打开连接

                // 构建基础查询语句
                var queryBuilder = new StringBuilder($"SELECT {selectClause} FROM {typeof(TEntity).Name}");

                // 如果有 where 子句，拼接到查询语句中
                if (!string.IsNullOrEmpty(whereClause))
                {
                    queryBuilder.Append(" WHERE ");
                    queryBuilder.Append(whereClause);
                }

                // 如果有 orderBy 子句，拼接到查询语句中
                if (!string.IsNullOrEmpty(orderBy))
                {
                    queryBuilder.Append($" ORDER BY {orderBy}");
                }

                // 如果有分页参数，拼接到查询语句中
                if (skip.HasValue)
                {
                    queryBuilder.Append($" LIMIT {skip.Value}, {take.GetValueOrDefault(int.MaxValue)}");
                }
                else if (take.HasValue)
                {
                    queryBuilder.Append($" LIMIT {take.Value}");
                }

                var query = queryBuilder.ToString();

                using (var command = new MySqlCommand(query, connection))
                {
                    // 添加查询参数
                    if (parameters != null)
                    {
                        foreach (var prop in parameters.GetType().GetProperties())
                        {
                            command.Parameters.AddWithValue($"@{prop.Name}", prop.GetValue(parameters));
                        }
                    }

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var entity = Activator.CreateInstance<TEntity>(); // 创建实体
                            Common.MapReaderToEntity<TEntity>(reader, entity); // 映射数据库字段到实体
                            entities.Add(entity);
                        }
                    }
                }
            }

            return entities;
        }

        public Task<List<TEntity>> QueryAsync<TEntity>() where TEntity : class
        {
            throw new NotImplementedException();
        }
    }
}

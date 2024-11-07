using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mfc.Core
{
    public class InMemoryDatabaseProvider : IDatabaseProvider
    {
        private readonly List<Dictionary<string, object>> _inMemoryDatabase = new List<Dictionary<string, object>>();

        public Task<int> ExecuteNonQueryAsync(string sql)
        {
            Console.WriteLine($"Executing NonQuery: {sql}");
            // 模拟执行的影响行数
            return Task.FromResult(1);
        }

        public Task<List<T>> ExecuteQueryAsync<T>(string sql)
        {
            Console.WriteLine($"Executing Query: {sql}");
            // 模拟查询结果
            return Task.FromResult(_inMemoryDatabase.Cast<T>().ToList());
        }

        public Task<int> ExecuteScalarAsync(string sql)
        {
            Console.WriteLine($"Executing Scalar: {sql}");
            return Task.FromResult(1); // 返回一个模拟的标量值
        }

        public Task<List<TEntity>> Query<TEntity>(string selectClause="*", string whereClause = null, object parameters = null, string orderBy = null, int? skip = null, int? take = null) where TEntity : class
        {
            throw new NotImplementedException();
        }
    }
}

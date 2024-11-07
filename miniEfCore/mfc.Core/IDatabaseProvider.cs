using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mfc.Core
{
    public interface IDatabaseProvider
    {
        Task<int> ExecuteNonQueryAsync(string sql);
        Task<List<T>> ExecuteQueryAsync<T>(string sql);
        Task<int> ExecuteScalarAsync(string sql);
        Task<List<TEntity>> Query<TEntity>(string selectClause = "*", string whereClause = null, object parameters = null, string orderBy = null, int? skip = null, int? take = null) where TEntity : class;
    }
}

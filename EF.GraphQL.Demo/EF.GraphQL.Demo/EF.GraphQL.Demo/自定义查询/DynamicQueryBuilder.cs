using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace EF.GraphQL.Demo
{
    public class DynamicQueryBuilder<TMain> where TMain : class
    {
        private readonly DbContext _dbContext;
        private IQueryable<TMain> _query;

        // 用于存储关联的分表信息
        private List<(string Alias, IQueryable<object> Table, string Condition)> _joins = new();

        public DynamicQueryBuilder(DbContext dbContext)
        {
            _dbContext = dbContext;
            _query = _dbContext.Set<TMain>();
        }

        public DynamicQueryBuilder<TMain> Where(string where,params object[] args)
        {
            _query = _query.Where(where, args);
            return this;
        }

        // 添加关联表 Join
        public DynamicQueryBuilder<TMain> Join<TSub>(string alias, Expression<Func<TMain, TSub, bool>> joinCondition)
            where TSub : class
        {
            var subTable = _dbContext.Set<TSub>().AsQueryable();
            _joins.Add((alias, subTable, joinCondition.ToString()));
            return this;
        }

        // 指定返回字段
        public DynamicQueryBuilder<TMain> Select(string fields)
        {
            _query = _query.Select<TMain>(fields);
            return this;
        }

        // 排序
        public DynamicQueryBuilder<TMain> OrderBy(string orderBy)
        {
            _query = _query.OrderBy(orderBy);
            return this;
        }

        // 分页
        public DynamicQueryBuilder<TMain> Paginate(int page, int pageSize)
        {
            _query = _query.Skip((page - 1) * pageSize).Take(pageSize);
            return this;
        }

        // 执行查询
        public async Task<List<object>> ExecuteAsync()
        {
            return await _query.ToDynamicListAsync();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mfc.Core
{
    public class InMemoryDatabaseProvider : IDatabaseProvider
    {
        private readonly Dictionary<Type, IList<object>> _inMemoryDatabase = new ();

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

            // 从内存数据库中获取 TEntity 类型的所有数据
            if (!_inMemoryDatabase.TryGetValue(typeof(TEntity), out var dataList))
            {
                return Task.FromResult(new List<TEntity>()); // 数据库中无此类型数据，返回空列表
            }

            // 将对象列表转换为 TEntity 类型
            var queryableData = dataList.Cast<TEntity>().AsQueryable();

            // 处理 whereClause 转换为 LINQ 表达式
            if (parameters != null && !string.IsNullOrEmpty(whereClause))
            {
                // 假设 whereClause 是 "PropertyName = @ParameterName" 形式
                var predicate = GeneratePredicate<TEntity>(whereClause, parameters);
                queryableData = (IQueryable<TEntity>)queryableData.Where(predicate);
            }

            // 处理排序
            if (!string.IsNullOrEmpty(orderBy))
            {
                queryableData = ApplyOrdering(queryableData, orderBy);
            }

            // 处理分页
            if (skip.HasValue)
            {
                queryableData = queryableData.Skip(skip.Value);
            }
            if (take.HasValue)
            {
                queryableData = queryableData.Take(take.Value);
            }

            // 执行查询并返回结果
            return Task.FromResult(queryableData.ToList());
        }

        private Func<TEntity, bool> GeneratePredicate<TEntity>(string whereClause, object parameters)
        {
            // 这是一个简单示例，仅处理 "Property = @Parameter" 类型的条件
            var parameterProperties = parameters.GetType().GetProperties();
            var predicates = whereClause.Split(new[] { " AND ", " OR " }, StringSplitOptions.None)
                                        .Select(condition =>
                                        {
                                            var parts = condition.Split(new[] { "=", ">", "<", ">=", "<=" }, StringSplitOptions.RemoveEmptyEntries);
                                            var propertyName = parts[0].Trim();
                                            var paramName = parts[1].Trim().TrimStart('@');

                                            var propertyInfo = typeof(TEntity).GetProperty(propertyName);
                                            var parameterValue = parameterProperties.FirstOrDefault(p => p.Name == paramName)?.GetValue(parameters);

                                            return new Func<TEntity, bool>(entity =>
                                            {
                                                var entityValue = propertyInfo?.GetValue(entity);
                                                return entityValue != null && entityValue.Equals(parameterValue);
                                            });
                                        });

            // 将多个条件组合为一个
            return entity => predicates.All(predicate => predicate(entity));
        }
        private IQueryable<TEntity> ApplyOrdering<TEntity>(IQueryable<TEntity> query, string orderBy)
        {
            var parts = orderBy.Split(' ');
            var propertyName = parts[0];
            var descending = parts.Length > 1 && parts[1].Equals("DESC", StringComparison.OrdinalIgnoreCase);

            var property = typeof(TEntity).GetProperty(propertyName);
            if (property == null)
            {
                throw new ArgumentException($"Property '{propertyName}' not found on type '{typeof(TEntity).Name}'");
            }

            // 使用 LINQ 动态排序
            return descending
                ? query.OrderByDescending(x => property.GetValue(x))
                : query.OrderBy(x => property.GetValue(x));
        }

    }
}

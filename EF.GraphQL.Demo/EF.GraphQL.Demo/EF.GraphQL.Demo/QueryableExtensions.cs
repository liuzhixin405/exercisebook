using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EF.GraphQL.Demo
{
    //public static class QueryableExtensions
    //{
    ////public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
    ////this IQueryable<T> query,
    ////QueryParameters parameters)
    ////{

    ////    foreach (var filter in parameters.Filters)
    ////    {
    ////        var propertyName = filter.Key;
    ////        var propertyValue = filter.Value.ToString();
    ////        if (string.IsNullOrEmpty(propertyName) || string.IsNullOrEmpty(propertyValue))
    ////        {
    ////            continue;
    ////        }
    ////        // 获取实体属性的表达式
    ////        var parameter = Expression.Parameter(typeof(T), "e");
    ////        var property = Expression.Property(parameter, propertyName);

    ////        // 将值转换为属性的类型
    ////        var propertyType = property.Type;
    ////        var typedValue = Convert.ChangeType(propertyValue, propertyType);

    ////        // 构建常量表达式，并确保类型匹配
    ////        var constant = Expression.Constant(typedValue, propertyType);

    ////        // 构建比较表达式 e.Property == Value
    ////        var equality = Expression.Equal(property, constant);

    ////        // 构建 Lambda 表达式
    ////        var lambda = Expression.Lambda<Func<T, bool>>(equality, parameter);
    ////        query = query.Where(lambda);
    ////    }

    ////    // 排序
    ////    if (!string.IsNullOrEmpty(parameters.OrderBy))
    ////    {
    ////        var propertyInfo = typeof(T).GetProperty(parameters.OrderBy);
    ////        if (propertyInfo != null)
    ////        {
    ////            var parameter = Expression.Parameter(typeof(T), "x");
    ////            var property = Expression.Property(parameter, propertyInfo);
    ////            var keySelector = Expression.Lambda(property, parameter);

    ////            query = parameters.IsDescending
    ////                ? Queryable.OrderByDescending(query, (dynamic)keySelector)
    ////                : Queryable.OrderBy(query, (dynamic)keySelector);
    ////        }
    ////    }

    ////    // 总记录数
    ////    var totalCount = await query.CountAsync();

    ////    // 分页
    ////    var items = await query
    ////        .Skip((parameters.PageIndex - 1) * parameters.PageSize)
    ////        .Take(parameters.PageSize)
    ////        .ToListAsync();

    ////    // 返回分页结果
    ////    return new PagedResult<T>
    ////    {
    ////        TotalCount = totalCount,
    ////        PageIndex = parameters.PageIndex,
    ////        PageSize = parameters.PageSize,
    ////        Items = items
    ////    };
    ////}
    //}

    public static class QueryableExtensions
    {
        public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
            this IQueryable<T> query,
            QueryParameters parameters)
        {
            // 动态筛选
            foreach (var filter in parameters.Filters)
            {
                var parameter = Expression.Parameter(typeof(T), "e");
                var property = Expression.Property(parameter, filter.FieldName);

                // 属性类型转换
                var propertyType = property.Type;
                var typedValue = Convert.ChangeType(filter.Value, propertyType);
                var constant = Expression.Constant(typedValue, propertyType);

                Expression condition = filter.Operator.ToLower() switch
                {
                    "like" when propertyType == typeof(string) =>
                        Expression.Call(property, typeof(string).GetMethod("Contains", new[] { typeof(string) }), constant),
                    ">=" => Expression.GreaterThanOrEqual(property, constant),
                    "<=" => Expression.LessThanOrEqual(property, constant),
                    ">" => Expression.GreaterThan(property, constant),
                    "<" => Expression.LessThan(property, constant),
                    "=" => Expression.Equal(property, constant),
                    _ => throw new NotSupportedException($"操作符 {filter.Operator} 不支持"),
                };

                var lambda = Expression.Lambda<Func<T, bool>>(condition, parameter);
                query = query.Where(lambda);
            }

            // 排序
            if (!string.IsNullOrEmpty(parameters.OrderBy))
            {
                var parameter = Expression.Parameter(typeof(T), "x");
                var property = Expression.Property(parameter, parameters.OrderBy);
                var keySelector = Expression.Lambda(property, parameter);

                query = parameters.IsDescending
                    ? Queryable.OrderByDescending(query, (dynamic)keySelector)
                    : Queryable.OrderBy(query, (dynamic)keySelector);
            }

            // 总记录数
            var totalCount = await query.CountAsync();

            // 分页
            var items = await query
                .Skip((parameters.PageIndex - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            // 返回分页结果
            return new PagedResult<T>
            {
                TotalCount = totalCount,
                PageIndex = parameters.PageIndex,
                PageSize = parameters.PageSize,
                Items = items
            };
        }
    }

}

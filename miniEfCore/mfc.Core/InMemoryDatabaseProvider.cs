using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace mfc.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    public class InMemoryDatabaseProvider : IDatabaseProvider
    {
        private readonly Dictionary<string, List<object>> _tables = new Dictionary<string, List<object>>();

        public Task<int> ExecuteNonQueryAsync(string sql)
        {
            var (operation, entity, condition) = ParseSql(sql);

            if (operation == "INSERT")
            {
                var tableName = entity.GetType().Name;
                if (!_tables.ContainsKey(tableName))
                    _tables[tableName] = new List<object>();

                _tables[tableName].Add(entity);
                return Task.FromResult(1);
            }
            else if (operation == "UPDATE")
            {
                return UpdateEntity(entity, condition);
            }
            else if (operation == "DELETE")
            {
                return DeleteEntity(condition);
            }

            return Task.FromResult(0);
        }

        public Task<List<T>> ExecuteQueryAsync<T>(string sql)
        {
            var tableName = typeof(T).Name;
            if (_tables.TryGetValue(tableName, out var records))
            {
                var result = records.Cast<T>().ToList();
                return Task.FromResult(result);
            }
            return Task.FromResult(new List<T>());
        }

        public Task<int> ExecuteScalarAsync(string sql)
        {
            // 示例实现，简单返回结果数量
            var (operation, entity, condition) = ParseSql(sql);
            var tableName = entity.GetType().Name;
            if (_tables.TryGetValue(tableName, out var records))
            {
                return Task.FromResult(records.Count);
            }
            return Task.FromResult(0);
        }

        public Task<int> UpdateEntity(object entity, Func<object, bool> condition)
        {
            var tableName = entity.GetType().Name;
            if (_tables.TryGetValue(tableName, out var records))
            {
                // 找到满足条件的第一个实体
                var recordToUpdate = records.FirstOrDefault(condition);
                if (recordToUpdate != null)
                {
                    // 更新现有实体的属性
                    UpdateEntityProperties(recordToUpdate, entity);
                    return Task.FromResult(1);
                }
            }
            return Task.FromResult(0);
        }

        public Task<int> DeleteEntity(Func<object, bool> condition)
        {
            int affectedRows = 0;
            foreach (var table in _tables.Values)
            {
                // 通过筛选条件删除满足条件的所有实体
                var entitiesToRemove = table.Where(condition).ToList();
                foreach (var entity in entitiesToRemove)
                {
                    table.Remove(entity);
                    affectedRows++;
                }
            }
            return Task.FromResult(affectedRows);
        }

        private void UpdateEntityProperties(object target, object source)
        {
            var properties = source.GetType().GetProperties();
            foreach (var property in properties)
            {
                var value = property.GetValue(source);
                property.SetValue(target, value);
            }
        }


        private (string operation, object entity, Func<object, bool> condition) ParseSql(string sql)
        {
            string operation = null;
            object entity = null;
            Func<object, bool> condition = null;

            if (sql.StartsWith("INSERT", StringComparison.OrdinalIgnoreCase))
            {
                operation = "INSERT";
                entity = ParseInsertStatement(sql);
            }
            else if (sql.StartsWith("UPDATE", StringComparison.OrdinalIgnoreCase))
            {
                operation = "UPDATE";
                (entity, condition) = ParseUpdateStatement(sql);
            }
            else if (sql.StartsWith("DELETE", StringComparison.OrdinalIgnoreCase))
            {
                operation = "DELETE";
                condition = ParseDeleteCondition(sql);
            }

            return (operation, entity, condition);
        }

        private object ParseInsertStatement(string sql)
        {
            // 假设 SQL 格式： "INSERT INTO User (Id, Name) VALUES (1, 'Alice')"
            var match = Regex.Match(sql, @"INSERT INTO (\w+)\s*\((.+?)\)\s*VALUES\s*\((.+?)\)", RegexOptions.IgnoreCase);
            if (!match.Success) return null;

            var tableName = match.Groups[1].Value;
            var columns = match.Groups[2].Value.Split(',').Select(c => c.Trim()).ToArray();
            var values = match.Groups[3].Value.Split(',').Select(v => v.Trim(' ', '\'')).ToArray();

            // 遍历所有加载的程序集来查找实体类型
            var entityType = AppDomain.CurrentDomain.GetAssemblies()
                                 .SelectMany(a => a.GetTypes())
                                 .FirstOrDefault(t => t.Name.Equals(tableName, StringComparison.OrdinalIgnoreCase));

            if (entityType == null)
            {
                throw new InvalidOperationException($"未找到表 '{tableName}' 对应的实体类型。");
            }

            var entity = Activator.CreateInstance(entityType);
            for (int i = 0; i < columns.Length; i++)
            {
                var property = entityType.GetProperty(columns[i], BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (property != null)
                {
                    var convertedValue = Convert.ChangeType(values[i], property.PropertyType);
                    property.SetValue(entity, convertedValue);
                }
            }

            return entity;
        }



        private (object entity, Func<object, bool> condition) ParseUpdateStatement(string sql)
        {
            var match = Regex.Match(sql, @"UPDATE (\w+)\s*SET\s*(.+?)\s*WHERE\s+(.+)", RegexOptions.IgnoreCase);
            if (!match.Success) return (null, null);

            var tableName = match.Groups[1].Value;
            var updates = match.Groups[2].Value.Split(',')
                .Select(u => u.Split('='))
                .ToDictionary(u => u[0].Trim(), u => u[1].Trim(' ', '\''));

            var entityType = Type.GetType(tableName);
            var entity = Activator.CreateInstance(entityType);
            foreach (var update in updates)
            {
                var property = entityType.GetProperty(update.Key);
                if (property != null)
                {
                    var convertedValue = Convert.ChangeType(update.Value, property.PropertyType);
                    property.SetValue(entity, convertedValue);
                }
            }

            var condition = GenerateCondition(entityType, match.Groups[3].Value);
            return (entity, condition);
        }

        private Func<object, bool> ParseDeleteCondition(string sql)
        {
            var match = Regex.Match(sql, @"DELETE FROM (\w+)\s*WHERE\s+(.+)", RegexOptions.IgnoreCase);
            if (!match.Success) return null;

            var tableName = match.Groups[1].Value;
            var entityType = Type.GetType(tableName);

            return GenerateCondition(entityType, match.Groups[2].Value);
        }

        private Func<object, bool> GenerateCondition(Type entityType, string condition)
        {
            var conditionParts = condition.Split('=');
            var propertyName = conditionParts[0].Trim();
            var value = conditionParts[1].Trim(' ', '\'');

            var property = entityType.GetProperty(propertyName);
            if (property == null) return null;

            return (object obj) =>
            {
                var entityValue = property.GetValue(obj);
                var convertedValue = Convert.ChangeType(value, property.PropertyType);
                return entityValue.Equals(convertedValue);
            };
        }

        public Task<List<TEntity>> Query<TEntity>(string selectClause = "*", string whereClause = null, object parameters = null, string orderBy = null, int? skip = null, int? take = null) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public Task InsertEntityAsync(object entity)
        {
            var tableName = entity.GetType().Name;
            if (!_tables.ContainsKey(tableName))
            {
                _tables[tableName] = new List<object>();
            }
            _tables[tableName].Add(entity);
            return Task.CompletedTask;
        }

        public Task<List<TEntity>> QueryAsync<TEntity>() where TEntity : class
        {
            var tableName = typeof(TEntity).Name;
            if (_tables.TryGetValue(tableName, out var records))
            {
                return Task.FromResult(records.Cast<TEntity>().ToList());
            }
            return Task.FromResult(new List<TEntity>());
        }

        public Task<TEntity> FindAsync<TEntity>(Func<TEntity, bool> predicate) where TEntity : class
        {
            var tableName = typeof(TEntity).Name;
            if (_tables.TryGetValue(tableName, out var records))
            {
                return Task.FromResult(records.Cast<TEntity>().FirstOrDefault(predicate));
            }
            return Task.FromResult<TEntity>(null);
        }

    }


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace mfc.Core
{
    public static class SqlGenerator
    {
        // 动态生成 INSERT SQL
        public static string GenerateInsertSql(object entity)
        {
            var entityType = entity.GetType();
            var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                       .Where(p => p.CanRead && p.GetValue(entity) != null)
                                       .ToList();

            var tableName = entityType.Name;
            var columns = string.Join(", ", properties.Select(p => p.Name));

            // 生成值的部分
            var values = string.Join(", ", properties.Select(p => FormatValue(p.GetValue(entity))));

            return $"INSERT INTO {tableName} ({columns}) VALUES ({values})";
        }

        private static string FormatValue(object value)
        {
            // 判断值的类型并返回正确的格式
            if (value is string || value is DateTime)
            {
                return $"'{value}'";  // 字符串和日期需要用单引号括起来
            }
            else if (value is bool)
            {
                return (bool)value ? "1" : "0";  // 布尔值转换为 1 或 0
            }
            else if (value is int || value is long || value is double || value is float || value is decimal)
            {
                return value.ToString();  // 数字类型不需要引号
            }
            else
            {
                return value.ToString();  // 默认其他类型调用 ToString()
            }
        }

        // 动态生成 UPDATE SQL
        public static string GenerateUpdateSql(object entity)
        {
            var entityType = entity.GetType();
            var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                       .Where(p => p.CanRead && p.GetValue(entity) != null && p.Name != "Id")
                                       .ToList(); // 假设 Id 是主键

            var tableName = entityType.Name;
            var setClause = string.Join(", ", properties.Select(p => $"{p.Name} = '{p.GetValue(entity)}'"));
            var idProperty = entityType.GetProperty("Id");
            var idValue = idProperty.GetValue(entity);

            return $"UPDATE {tableName} SET {setClause} WHERE Id = {idValue}";
        }

        // 动态生成 DELETE SQL
        public static string GenerateDeleteSql(object entity)
        {
            var entityType = entity.GetType();
            var idProperty = entityType.GetProperty("Id");
            var idValue = idProperty.GetValue(entity);

            return $"DELETE FROM {entityType.Name} WHERE Id = {idValue}";
        }
    }
}

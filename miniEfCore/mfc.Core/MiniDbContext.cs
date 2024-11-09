using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace mfc.Core
{
    public class MiniDbContext
    {
      
        private readonly EntityStateTracker _entityStateTracker = new EntityStateTracker();
        private readonly IDatabaseProvider _databaseProvider;

        //// 追踪实体
        //public void TrackEntity(object entity)
        //{
        //    // 默认为新实体，设置为 Added
        //    _entityStateTracker.SetAdded(entity);
            
        //    _entities.Add(entity);
        //}

        public MiniDbContext(IDatabaseProvider databaseProvider)
        {
            _databaseProvider = databaseProvider;
        }

        public DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return new DbSet<TEntity>(_entityStateTracker, _databaseProvider);
        }

        public async Task SaveChangesAsync()
        {
            Console.WriteLine("Saving changes...");
            // 从 EntityStateTracker 获取所有状态变化的实体
            var changedEntities = _entityStateTracker.GetChangedEntities();

            foreach (var entity in changedEntities)
            {
                // 直接通过状态判断实体的操作
                var state = _entityStateTracker.GetEntityState(entity);

                // 检查当前的数据库提供者类型
                if (_databaseProvider is InMemoryDatabaseProvider inMemoryProvider)
                {
                    // 直接操作内存表而非生成 SQL
                    if (state == EntityState.Added)
                    {
                        await inMemoryProvider.InsertEntityAsync(entity);
                    }
                    else if (state == EntityState.Modified)
                    {
                       await  inMemoryProvider.UpdateEntity(entity, e => e.Equals(entity));
                    }
                    else if (state == EntityState.Deleted)
                    {
                        await inMemoryProvider.DeleteEntity(e => e.Equals(entity));
                    }
                }
                else
                {
                    if (state == EntityState.Added)
                    {
                        var insertSql = SqlGenerator.GenerateInsertSql(entity);
                        Console.WriteLine($"INSERT SQL: {insertSql}");
                        await _databaseProvider.ExecuteNonQueryAsync(insertSql);
                    }
                    else if (state == EntityState.Modified)
                    {
                        var updateSql = SqlGenerator.GenerateUpdateSql(entity);
                        Console.WriteLine($"UPDATE SQL: {updateSql}");
                        await _databaseProvider.ExecuteNonQueryAsync(updateSql);
                    }
                    else if (state == EntityState.Deleted)
                    {
                        var deleteSql = SqlGenerator.GenerateDeleteSql(entity);
                        Console.WriteLine($"DELETE SQL: {deleteSql}");
                        await _databaseProvider.ExecuteNonQueryAsync(deleteSql);
                    }
                }
            }

            // 清空变更列表
            _entityStateTracker.ClearChangedEntities();
        }

        public async Task ExecuteInTransactionAsync(Func<Task> action)
        {
            try
            {
                await _databaseProvider.ExecuteNonQueryAsync("BEGIN TRANSACTION");
                await action();
                await _databaseProvider.ExecuteNonQueryAsync("COMMIT");
            }
            catch (Exception)
            {
                await _databaseProvider.ExecuteNonQueryAsync("ROLLBACK");
                throw;
            }
        }

        // 动态生成 INSERT SQL
        public static string GenerateInsertSql(object entity)
        {
            var entityType = entity.GetType();
            var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                       .Where(p => p.CanRead && p.GetValue(entity) != null)
                                       .ToList();

            var tableName = entityType.Name;
            var columns = string.Join(", ", properties.Select(p => p.Name));
            var values = string.Join(", ", properties.Select(p => $"'{p.GetValue(entity)}'"));

            return $"INSERT INTO {tableName} ({columns}) VALUES ({values})";
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

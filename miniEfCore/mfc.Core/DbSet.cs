using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mfc.Core
{
    public class DbSet<TEntity> where TEntity : class
    {
        private readonly IDatabaseProvider _databaseProvider;
        private readonly List<TEntity> _entities;
        private readonly EntityStateTracker _entityStateTracker;

        // 构造函数
        public DbSet(IDatabaseProvider databaseProvider, List<TEntity> entities, EntityStateTracker entityStateTracker)
        {
            _databaseProvider = databaseProvider;
            _entities = entities ?? new List<TEntity>();
            _entityStateTracker = entityStateTracker;
        }

        // 添加实体
        public void Add(TEntity entity)
        {
            _entities.Add(entity);
            _entityStateTracker.SetAdded(entity);
        }

        // 更新实体
        public void Update(TEntity entity)
        {
            if (_entities.Contains(entity))
            {
                _entityStateTracker.SetModified(entity);
            }
            else
            {
                _entities.Add(entity);
                _entityStateTracker.SetAdded(entity);
            }
        }

        // 删除实体
        public void Remove(TEntity entity)
        {
            if (_entities.Contains(entity))
            {
                _entities.Remove(entity);
                _entityStateTracker.SetDeleted(entity);
            }
        }

        // 获取所有实体
        public List<TEntity> ToList()
        {
            // 查询条件只会在需要时传递给数据库
            var dbEntities = _databaseProvider.Query<TEntity>().Result;  // 根据条件查询数据
            return dbEntities;
        }

        // 查找符合条件的实体
        public IEnumerable<TEntity> Where(Func<TEntity, bool> predicate)
        {
            //return _entities.Where(predicate);
            // 查询条件只会在需要时传递给数据库
            var dbEntities = _databaseProvider.Query<TEntity>().Result;  // 根据条件查询数据
            return dbEntities.Where(predicate);
        }
    }
}
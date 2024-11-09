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
     
        private readonly List<TEntity> _entities=new();
        private readonly EntityStateTracker _entityStateTracker;

        private readonly IDatabaseProvider _dataProvider;
        // 构造函数
        public DbSet( EntityStateTracker entityStateTracker, IDatabaseProvider dataProvider)
        {
            _entityStateTracker = entityStateTracker;
            _dataProvider = dataProvider;
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


        // 获取所有实体，使用数据提供者查询
        public async Task<List<TEntity>> ToListAsync()
        {
            return await _dataProvider.QueryAsync<TEntity>();
        }

        // 根据条件查找实体
        public async Task<TEntity> FindAsync(Func<TEntity, bool> predicate)
        {
            return await _dataProvider.FindAsync(predicate);
        }
    }
}
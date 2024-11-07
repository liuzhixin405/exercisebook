using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mfc.Core
{
    public class EntityStateTracker
    {
        private readonly Dictionary<object, EntityState> _entityStates = new Dictionary<object, EntityState>();

        public void TrackEntity(object entity,EntityState state)
        {
            if (!_entityStates.ContainsKey(entity))
            {
                _entityStates.Add(entity, state);
            }
            else
            {
                _entityStates[entity] = state;
            }
        }
        // 获取实体的当前状态
        public EntityState GetEntityState(object entity)
        {
            return _entityStates.ContainsKey(entity) ? _entityStates[entity] : EntityState.Detached;
        }

        // 设置为新增
        public void SetAdded(object entity)
        {
            TrackEntity(entity, EntityState.Added);
        }

        // 设置为修改
        public void SetModified(object entity)
        {
            TrackEntity(entity, EntityState.Modified);
        }

        // 设置为删除
        public void SetDeleted(object entity)
        {
            TrackEntity(entity, EntityState.Deleted);
        }

        // 清除所有已变更的实体
        public void ClearChangedEntities()
        {
            _entityStates.Clear();
        }

        // 获取所有已变更的实体
        public List<object> GetChangedEntities()
        {
            return _entityStates.Where(e => e.Value != EntityState.Unchanged)
                                 .Select(e => e.Key)
                                 .ToList();
        }
    }
}

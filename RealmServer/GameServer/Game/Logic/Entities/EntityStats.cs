using Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game.Logic.Entities
{
    public class EntityStats
    {
        private readonly object _statsLock = new object();

        public event Action<Entity, StatType, object> StatChangedEvent;

        private readonly Entity _entity;
        private readonly Dictionary<StatType, object> _stats;
        private ObjectDropData _objDropData;
        private ObjectData _objData;
        private bool _updateStatus = true;

        public EntityStats(Entity host)
        {
            _entity = host;

            _stats = new Dictionary<StatType, object>();
        }

        public T Get<T>(StatType statType)
        {
            lock (_statsLock)
            {
                if (!_stats.TryGetValue(statType, out var ret))
                    return default;
                return (T)ret;
            }
        }

        public void Set(StatType statType, object value)
        {
            lock (_statsLock)
            {
                if (_stats.TryGetValue(statType, out _))
                    _stats[statType] = value;
                else
                    _stats.TryAdd(statType, value);
            }

            _updateStatus = true;
            StatChangedEvent?.Invoke(_entity, statType, value);
        }

        public void UpdatePosition()
        {
            StatChangedEvent?.Invoke(_entity, StatType.None, 0);
        }

        public ObjectData GetObjectData()
        {
            _objData.ObjectType = _entity.Desc.ObjectType;
            _objData.Status.ObjectId = _entity.Id;
            _objData.Status.Pos = _entity.Position;
            _objData.Status.Stats ??= new List<StatData>();

            if (_updateStatus)
            {
                _updateStatus = false;

                lock (_statsLock)
                    _objData.Status.Stats = _stats.Select(kvp => new StatData(kvp.Key, kvp.Value)).ToList();
            }

            return _objData;
        }

        public ObjectDropData GetObjectDropData()
        {
            _objDropData.ObjectId = _entity.Id;
            if (Get<int>(StatType.HP) <= 0)
                _objDropData.Explode = true;

            return _objDropData;
        }

        public void Clear()
        {
            _stats.Clear();
            StatChangedEvent = null;
        }
    }
}

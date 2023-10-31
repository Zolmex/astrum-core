using Common.Utilities;
using GameServer.Game.Logic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game.Logic
{
    public class EntityCollection : SmartCollection<Entity>
    {
        private int _worldId;

        public EntityCollection(int worldId)
        {
            _worldId = worldId;
        }

        public override void Update()
        {
            while (_adds.TryDequeue(out var newEntity))
                lock (_dict)
                    if (_dict.TryAdd(newEntity.Id, newEntity))
                    {
                        Count++;
                        newEntity.Initialize();
                    }

            while (_drops.TryDequeue(out var oldEntityId))
                lock (_dict)
                    if (_dict.Remove(oldEntityId, out var oldEntity))
                    {
                        Count--;
                        oldEntity.Dispose();
                    }
        }
    }
}

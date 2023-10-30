using Common.Utilities;
using GameServer.Game.Logic.Worlds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game.Logic
{
    public class WorldCollection : SmartCollection<World>
    {
        private static readonly Logger _log  = new Logger(typeof(WorldCollection));

        public override void Update()
        {
            while (_adds.TryDequeue(out var newWorld))
                if (_dict.TryAdd(newWorld.Id, newWorld))
                {
                    Count++;
                    newWorld.Initialize();
                    _log.Info($"World {newWorld.Name}({newWorld.Id}) added.");
                }

            while (_drops.TryDequeue(out var oldWorldId))
                if (_dict.Remove(oldWorldId, out var oldWorld))
                {
                    Count--;
                    oldWorld.Dispose();
                    _log.Info($"World {oldWorld.Name}({oldWorld.Id}) removed.");
                }
        }
    }
}

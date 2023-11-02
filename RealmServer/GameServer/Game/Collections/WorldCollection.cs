using Common.Resources.Xml;
using Common.Utilities;
using GameServer.Game.Worlds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game.Collections
{
    public class WorldCollection : SmartCollection<World>
    {
        private static readonly Logger _log = new Logger(typeof(WorldCollection));
        private readonly Dictionary<string, World> _worldNames = new Dictionary<string, World>();

        public override void Update()
        {
            while (_adds.TryDequeue(out var newWorld))
                lock (_dict)
                    if (_dict.TryAdd(newWorld.Id, newWorld))
                    {
                        Count++;
                        _worldNames.TryAdd(newWorld.Name, newWorld);

                        newWorld.Initialize();
                        _log.Info($"World {newWorld.Name}({newWorld.Id}) added.");
                    }

            while (_drops.TryDequeue(out var oldWorldId))
                lock (_dict)
                    if (_dict.Remove(oldWorldId, out var oldWorld))
                    {
                        Count--;
                        _worldNames.Remove(oldWorld.Name, out _);

                        oldWorld.Dispose();
                        _log.Info($"World {oldWorld.Name}({oldWorld.Id}) removed.");
                    }
        }

        public World Get(string name)
        {
            lock (_dict)
            {
                if (!_worldNames.TryGetValue(name, out var ret))
                    return default;
                return ret;
            }
        }
    }
}

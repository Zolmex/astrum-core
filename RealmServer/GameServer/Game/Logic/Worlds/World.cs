using Common;
using Common.Resources.World;
using Common.Utilities;
using GameServer.Game.Logic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace GameServer.Game.Logic.Worlds
{
    public class World : IIdentifiable
    {
        public const string NEXUS = "Nexus";
        public const int UNBLOCKED_SIGHT = 0;

        public int Id { get; set; }
        public string Name { get; private set; }

        public bool Alive { get; private set; }
        public bool Disposed { get; private set; }
        public bool Deleted { get; private set; }
        public WorldConfig Config { get; private set; }

        public WorldMap Map { get; private set; }
        public int MapId { get; private set; }

        private static int _nextWorldId;

        public readonly EntityCollection Entities; // Complete list of entities in this world
        public readonly PlayerCollection Players;
        private readonly Logger _log;

        private long _startTime;

        public World(string name, int mapId, int worldId = 0)
        {
            _log = new Logger(GetType());

            Id = worldId == 0 ? Interlocked.Increment(ref _nextWorldId) : worldId;
            Name = name;
            MapId = mapId;
            Config = WorldLibrary.WorldConfigs[name];

            Entities = new EntityCollection(Id);
            Players = new PlayerCollection();
        }

        public void Initialize()
        {
            Alive = true;

            LoadMap(Config.Name, MapId);

            _startTime = RealmManager.GlobalTime.TotalElapsedMs;
        }

        public void LoadMap(string mapName, int mapId)
        {
            var maps = WorldLibrary.JsonMaps[mapName]; // Load our json maps

            if (maps.Length > 0)
            {
                var jsonMap = maps[mapId];

                Map = new WorldMap(jsonMap);

                foreach (var en in Map.Entities) // Load entities from the map
                    en.EnterWorld(this);
            }
        }

        public void AddEntity(Entity en) // Should always call Entity.EnterWorld, don't call this directly
        {
            Entities.Add(en);

            if (en is Player player)
                Players.Add(player);
        }

        public void RemoveEntity(Entity en)
        {
            var entityId = en.Id;
            Entities.Remove(entityId);

            if (en is Player)
                Players.Remove(entityId);
        }

        public void Update() // Different from Tick, synchronizes collections and other stuff maybe
        {
            Entities.Update();
            Players.Update();
        }

        public void Tick(RealmTime time)
        {
            if (Deleted)
                return;

            if (!Config.LongLasting && Players.Count == 0 && (time.TotalElapsedMs - _startTime) > 60000)
            {
                Delete();
                return;
            }

            Parallel.ForEach(Entities, kvp =>
            {
                var en = kvp.Value;
                if (!en.Dead)
                    en.Tick(time);
            });
        }

        public void Delete()
        {
            Deleted = true;

            foreach (var kvp in Entities)
                kvp.Value.Death();

            Update(); // Perform one last time

            RealmManager.RemoveWorld(Id);
        }

        public void Dispose()
        {
            Disposed = true;
            Entities.Dispose();
            Players.Dispose();
        }
    }
}

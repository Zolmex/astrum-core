using Common;
using Common.Resources.World;
using Common.Utilities;
using GameServer.Game.Entities;
using GameServer.Game.Logic;
using GameServer.Game.Logic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace GameServer.Game.Worlds
{
    public class World : IIdentifiable
    {
        public const string NEXUS = "Nexus";
        public const int NEXUS_ID = -1;
        public const string REALM = "Realm";
        public const int UNBLOCKED_SIGHT = 0;

        public int Id { get; set; }
        public string Name { get; protected set; }

        public bool Disposed { get; private set; }
        public bool Deleted { get; private set; }
        public WorldConfig Config { get; private set; }

        public WorldMap Map { get; private set; }
        public int MapId { get; private set; }

        private static int _nextWorldId;

        public readonly EntityCollection Entities; // Complete list of entities in this world
        public readonly CharacterCollection Characters;
        public readonly PlayerCollection Players;
        public readonly SmartCollection<Entity> ActiveEntities;

        protected readonly Logger _log;

        private long _startTime;

        public World(string name, int mapId, int worldId = 0)
        {
            _log = new Logger(GetType());

            Id = worldId == 0 ? Interlocked.Increment(ref _nextWorldId) : worldId;
            Name = name;
            MapId = mapId;
            Config = WorldLibrary.WorldConfigs[name];

            Entities = new EntityCollection();
            Characters = new CharacterCollection();
            Players = new PlayerCollection();
            ActiveEntities = new SmartCollection<Entity>();
        }

        public virtual void Initialize()
        {
            LoadMap(Config.Name, MapId);

            _startTime = RealmManager.GlobalTime.TotalElapsedMs;
        }

        public void LoadMap(string mapName, int mapId)
        {
            var maps = WorldLibrary.MapDatas[mapName]; // Load our json maps

            if (maps.Length > 0)
            {
                if (mapId == -1)
                    mapId = Random.Shared.Next(maps.Length - 1);

                var jsonMap = maps[mapId];

                Map = new WorldMap(jsonMap);

                foreach (var en in Map.Entities) // Load entities from the map
                    en.EnterWorld(this);
            }
        }

        public void AddEntity(Entity en) // Should always call Entity.EnterWorld, don't call this directly
        {
            Entities.Add(en);

            if (en is Character chr)
                Characters.Add(chr);

            if (en is Player player)
                Players.Add(player);
        }

        public void RemoveEntity(Entity en)
        {
            var entityId = en.Id;
            Entities.Remove(entityId);

            if (en is Character)
                Characters.Remove(entityId);

            if (en is Player)
                Players.Remove(entityId);
        }

        public void Update() // Different from Tick, synchronizes collections and other stuff maybe
        {
            Entities.Update();
            Characters.Update();
            Players.Update();
        }

        public void Tick(RealmTime time)
        {
            if (Deleted)
            {
                foreach (var kvp in Entities)
                    kvp.Value.LeaveWorld();

                Update(); // Perform one last time

                RealmManager.RemoveWorld(Id);
                return;
            }

            if (!Config.LongLasting && Players.Count == 0 && time.TotalElapsedMs - _startTime > 60000)
            {
                Delete();
                return;
            }

            for (var cY = 0; cY < Map.Chunks.Height; cY++)
                for (var cX = 0; cX < Map.Chunks.Width; cX++)
                {
                    var chunk = Map.Chunks[cX, cY];
                    if (chunk.Activity == 0)
                        continue;

                    lock (chunk.Entities)
                        foreach (var en in chunk.Entities)
                            if (!en.Dead) // Here we put into a list the entities that belong to active chunks
                            {
                                ActiveEntities.Add(en);
                                if (en is Character chr && en is not Player)
                                    chr.Tick(time);
                            }
                }

            ActiveEntities.Update();

            Parallel.ForEach(Players, kvp => // Players are ticked after to ensure that the active entities list is complete
            {
                var plr = kvp.Value;
                if (!plr.Dead)
                    plr.Tick(time);
            });

            ActiveEntities.Clear();
        }

        public void Delete()
        {
            Deleted = true; // The actual deletion of the world is performed in Tick
        }

        public void Dispose()
        {
            Disposed = true;
            ActiveEntities.Clear();
            Entities.Clear();
            Characters.Clear();
            Players.Clear();
        }
    }
}

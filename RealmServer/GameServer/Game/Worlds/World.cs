using Common;
using Common.Resources.World;
using Common.Utilities;
using GameServer.Game.Entities;
using GameServer.Game.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Common.Resources.Config;

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

        public Thread LogicThread { get; private set; }
        public LogicTicker Logic { get; }

        public bool Disposed { get; private set; }
        public bool Deleted { get; private set; }
        public WorldConfig Config { get; private set; }

        public WorldMap Map { get; private set; }
        public int MapId { get; private set; }

        private static int _nextWorldId;

        public readonly EntityCollection Entities; // Complete list of entities in this world
        public readonly CharacterCollection Characters;
        public readonly CharacterCollection Enemies;
        public readonly PlayerCollection Players;
        public readonly SmartCollection<Entity> ActiveEntities;

        private readonly object _playerTickLock = new object();
        private event Action<Player> _playerTick;

        protected readonly Logger _log;

        private long _startTime;

        public World(string name, int mapId, int worldId = 0)
        {
            _log = new Logger(GetType());

            Id = worldId == 0 ? Interlocked.Increment(ref _nextWorldId) : worldId;
            Name = name;
            MapId = mapId;
            Config = WorldLibrary.WorldConfigs[name];
            Logic = new LogicTicker(this);

            Entities = new EntityCollection();
            Characters = new CharacterCollection();
            Enemies = new CharacterCollection();
            Players = new PlayerCollection();
            ActiveEntities = new SmartCollection<Entity>();
        }

        public virtual void Initialize()
        {
            LoadMap(Config.Name, MapId);

            _startTime = RealmManager.GlobalTime.TotalElapsedMs;

            if (Config.LongLasting)
            {
                _startTime = 0;

                LogicThread = new Thread(() => Logic.Run(GameServerConfig.Config.MsPT));
                LogicThread.Name = $"[{Id}]{Name}";
                LogicThread.Start();
            }
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
            {
                Characters.Add(chr);

                if (en is Enemy enemy)
                    Enemies.Add(enemy);
            }

            if (en is Player player)
                Players.Add(player);
        }

        public void RemoveEntity(Entity en)
        {
            var entityId = en.Id;
            Entities.Remove(entityId);

            if (en is Character)
            {
                Characters.Remove(entityId);

                if (en is Enemy)
                    Enemies.Remove(entityId);
            }

            if (en.IsPlayer)
                Players.Remove(entityId);
        }

        public void Update() // Different from Tick, synchronizes collections and other stuff maybe
        {
            Entities.Update();
            Characters.Update();
            Enemies.Update();
            Players.Update();
        }

        public virtual void Tick(RealmTime time)
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
                                if (!en.IsPlayer)
                                    ActiveEntities.Add(en);

                                if (en is Character chr && !en.IsPlayer)
                                    chr.Tick(time);
                            }
                }

            ActiveEntities.Update();

            Parallel.ForEach(Players, kvp => // Players are ticked after to ensure that the active entities list is complete
            {
                var plr = kvp.Value;
                if (!plr.Dead)
                {
                    lock (_playerTickLock)
                        _playerTick?.Invoke(plr);
                    plr.Tick(time);
                }
            });

            lock (_playerTickLock)
                _playerTick = null;

            ActiveEntities.Clear();
        }

        public bool IsPassable(double x, double y, bool spawning = false, bool bypassNoWalk = false)
        {
            var x_ = (int)x;
            var y_ = (int)y;

            if (!Map.Contains(x_, y_))
                return false;

            var tile = Map[x_, y_];
            if (tile.TileDesc.NoWalk && !bypassNoWalk)
                return false;

            if (tile.ObjectType == 0 || tile.Object == null)
                return true;

            return !tile.Object.Desc.FullOccupy && !tile.Object.Desc.EnemyOccupySquare && (!spawning || !tile.Object.Desc.OccupySquare);
        }

        public Entity SpawnSetPiece(string spName, int spawnX, int spawnY, int mapIndex = -1, string eventId = null)
        {
            if (spawnX < 0 || spawnY < 0 || spawnX > Map.Width || spawnY > Map.Height)
                return null;

            if (!WorldLibrary.MapDatas.TryGetValue(spName, out var setpiece))
            {
                _log.Error($"Invalid setpiece: {spName}");
                return null;
            }

            Entity ret = null;
            var map = mapIndex == -1 ? setpiece.RandomElement() : setpiece[mapIndex];
            for (var spY = 0; spY < map.Height; spY++)
                for (var spX = 0; spX < map.Width; spX++)
                {
                    var x = spawnX + spX;
                    var y = spawnY + spY;
                    if (x < 0 || y < 0 || x > Map.Width || y > Map.Height)
                        continue;

                    var tile = Map[x, y];
                    var spTile = map.Tiles[spX, spY];
                    if (spTile.GroundType != 255)
                    {
                        tile.Object?.LeaveWorld();
                        tile.Update(spTile);
                    }

                    if (spTile.ObjectType != 0xff && spTile.ObjectType != 0)
                    {
                        tile.Object?.LeaveWorld();

                        var entity = Entity.Resolve(spTile.ObjectType);
                        if (entity.Desc.Static)
                        {
                            if (entity.Desc.BlocksSight)
                                tile.BlocksSight = true;
                            tile.Object = entity;
                        }

                        entity.Move(x + 0.5f, y + 0.5f);
                        entity.EnterWorld(this);

                        if (!string.IsNullOrEmpty(eventId) && eventId == entity.Desc.ObjectId)
                            ret = entity;
                    }

                    if (tile.Region != TileRegion.None)
                    {
                        var pos = new WorldPosData() { X = x, Y = y };
                        Map.Regions[pos] = tile.Region;
                    }

                    BroadcastAll(plr => plr.TileUpdate(tile));
                }

            return ret;
        }

        public void BroadcastAll(Action<Player> act)
        {
            lock (_playerTickLock)
                _playerTick += act;
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
            Enemies.Clear();
            Players.Clear();
        }
    }
}

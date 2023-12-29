using Common.Resources.World;
using Common;
using Common.Resources.Xml;
using Common.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Threading;
using GameServer.Game.Worlds;
using GameServer.Game.Collections;

namespace GameServer.Game.Entities
{
    public class Entity : IIdentifiable
    {
        private static readonly Random _random = new Random();

        protected readonly Logger _log;
        private static int _nextEntityId;

        #region STATS

        public string Name { get => Stats.Get<string>(StatType.Name); set => Stats.Set(StatType.Name, value); }
        public int MaxHP { get => Stats.Get<int>(StatType.MaxHP); set => Stats.Set(StatType.MaxHP, value); }
        public int HP { get => Stats.Get<int>(StatType.HP); set => Stats.Set(StatType.HP, value); }
        public int Size { get => Stats.Get<int>(StatType.Size); set => Stats.Set(StatType.Size, value); }

        #endregion

        public int Id { get; set; }
        public ObjectDesc Desc { get; }
        public EntityStats Stats { get; }
        public WorldTile Tile { get; private set; }
        public World World { get; private set; }
        public bool Dead { get; private set; }

        protected bool _initialized;
        public WorldPosData Position;
        public readonly bool IsPlayer;
        public event Action<Entity> DeathEvent;
        protected readonly object _deathLock = new object();
        protected readonly object _dmgLock = new object();

        public Entity(ushort type)
        {
            _log = new Logger(GetType());

            Id = Interlocked.Increment(ref _nextEntityId);
            Desc = XmlLibrary.ObjectDescs[type];
            IsPlayer = Desc.Class == "Player";
            Stats = new EntityStats(this);
        }

        public static Entity Resolve(ushort objType)
        {
            var desc = XmlLibrary.ObjectDescs[objType];

            if (desc.Class != null)
                switch (desc.Class)
                {
                    case "Portal": // Dungeon portals
                        var dungeonName = desc.DungeonName;
                        if (dungeonName != null)
                            return new Portal(objType);
                        break;
                    case "Character":
                        if (desc.Enemy)
                            return new Enemy(objType);
                        return new Character(objType);
                }
            //if (desc.ConnectedWall || desc.CaveWall)
            //    return new ConnectedObject(objType);
            //if (desc.Static)
            //    return new StaticObject(objType);
            //if (desc.Enemy)
            //    return new Enemy(objType);
            //if (desc.Class == "Character")
            //    return new Character(objType);
            return new Entity(objType);
        }

        public void EnterWorld(World world)
        {
            World = world;
            World.AddEntity(this);

            Tile = world.Map[(int)Position.X, (int)Position.Y];
            Tile.Chunk.Insert(this);
        }

        public virtual void Initialize()
        {
            LoadStats();
            _initialized = true;
        }

        protected virtual void LoadStats()
        {
            Name = Desc.ObjectId;
            MaxHP = Desc.MaxHP;
            HP = MaxHP;
            Size = Desc.MaxSize != 0 ? _random.Next(Desc.MinSize, Desc.MaxSize) : Desc.Size;
        }

        public virtual bool Move(float posX, float posY)
        {
            if (Dead)
                return false;

            Position.X = posX;
            Position.Y = posY;

            if (World != null && posX >= 0 && posX < World.Map.Width && posY >= 0 && posY < World.Map.Height)
            {
                var oldChunk = Tile?.Chunk;
                Tile = World.Map[(int)posX, (int)posY];

                if (Tile.Chunk != oldChunk)
                {
                    var newChunk = Tile?.Chunk;
                    World.OnUpdate(() =>
                    {
                        oldChunk?.Remove(this);
                        newChunk.Insert(this);
                    });
                }
            }

            Stats.UpdatePosition();
            return true;
        }

        public virtual void LeaveWorld()
        {
            lock (_deathLock)
            {
                if (Dead)
                    return;

                Dead = true;
                DeathEvent?.Invoke(this);
                DeathEvent = null;

                World.RemoveEntity(this);
            }
        }

        public void ProjectileHit(Character from, int bulletId)
        {
            if (from.Dead)
                return;

            if (bulletId > 255)
                return;

            var proj = from.Projectiles[bulletId];
            if (proj == null || proj.Dead)
            {
                var message = proj == null ? $"BulletId:{bulletId}" : $"TimeAlive:{proj.TimeAlive} TTL:{proj.TTL}";
                _log.Warn($"PROJ HIT FAILED: {message}");
                return;
            }

            Damage(proj.Damage);
            proj.HitAdd(this);

            if (HP <= 0)
                Death();
        }

        public void Damage(int baseDamage)
        {
            int damage = baseDamage;

            //apply dmg reductions, dmg modifiers, etc

            lock (_dmgLock)
                HP -= damage;
        }

        public virtual void Death()
        {
            LeaveWorld();
        }

        public virtual void Dispose()
        {
            Tile = null;
            World = null;
            Stats.Clear();
        }
    }
}

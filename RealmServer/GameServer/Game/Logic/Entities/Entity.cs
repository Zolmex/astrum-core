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
using GameServer.Game.Logic.Worlds;

namespace GameServer.Game.Logic.Entities
{
    public class Entity : IIdentifiable
    {
        private static readonly Random _random = new Random();

        protected readonly Logger _log;
        private static int _nextEntityId;

        public int Id { get; set; }
        public ObjectDesc Desc { get; }
        public EntityStats Stats { get; }

        public WorldTile Tile;
        public WorldPosData Position;
        public event Action<Entity> DeathEvent;

        #region STATS

        public string Name { get => Stats.Get<string>(StatType.Name); set => Stats.Set(StatType.Name, value); }
        public int MaxHP { get => Stats.Get<int>(StatType.MaxHP); set => Stats.Set(StatType.MaxHP, value); }
        public int HP { get => Stats.Get<int>(StatType.HP); set => Stats.Set(StatType.HP, value); }
        public int Size { get => Stats.Get<int>(StatType.Size); set => Stats.Set(StatType.Size, value); }

        #endregion

        public World World { get; private set; }
        public bool Dead { get; private set; }

        protected RealmTime _lastTick;
        protected readonly object _deathLock = new object();

        public Entity(ushort type)
        {
            _log = new Logger(GetType());

            Id = Interlocked.Increment(ref _nextEntityId);
            Desc = XmlLibrary.ObjectDescs[type];
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
        }

        public virtual void Initialize()
        {
            LoadStats();
        }

        protected virtual void LoadStats()
        {
            Name = Desc.ObjectId;
            MaxHP = Desc.MaxHP;
            HP = MaxHP;
            Size = Desc.MaxSize != 0 ? _random.Next(Desc.MinSize, Desc.MaxSize) : Desc.Size;
        }

        public virtual void Move(float posX, float posY)
        {
            Position.X = posX;
            Position.Y = posY;

            if (World != null && posX >= 0 && posX < World.Map.Width && posY >= 0 && posY < World.Map.Height)
                Tile = World.Map[(int)posX, (int)posY];

            Stats.UpdatePosition();
        }

        public virtual bool Tick(RealmTime time)
        {
            if (Dead || _lastTick.TickCount == time.TickCount) // Ensure that ticks only occur once per logic tick
                return false;

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

        public virtual void Dispose()
        {
            World = null;
            Stats.Clear();
        }
    }
}

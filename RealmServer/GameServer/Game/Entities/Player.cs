using Common;
using Common.Database;
using Common.Resources.World;
using Common.Utilities;
using GameServer.Game.Entities;
using GameServer.Game.Network.Messaging;
using GameServer.Game.Network.Messaging.Outgoing;
using GameServer.Game.Worlds;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game.Entities
{
    public partial class Player : Character
    {
        public User User { get; }
        public DbChar Char { get; }
        public PlayerInventory Inventory { get; }

        #region STATS

        public int MaxMP { get => Stats.Get<int>(StatType.MaxMP); set => Stats.Set(StatType.MaxMP, value); }
        public int MP { get => Stats.Get<int>(StatType.MP); set => Stats.Set(StatType.MP, value); }
        public int Experience { get => Stats.Get<int>(StatType.Experience); set => Stats.Set(StatType.Experience, value); }
        public int Level { get => Stats.Get<int>(StatType.Level); set => Stats.Set(StatType.Level, value); }
        public int NumStars { get => Stats.Get<int>(StatType.NumStars); set => Stats.Set(StatType.NumStars, value); }
        public int Attack { get => Stats.Get<int>(StatType.Attack); set => Stats.Set(StatType.Attack, value); }
        public int Defense { get => Stats.Get<int>(StatType.Defense); set => Stats.Set(StatType.Defense, value); }
        public int Speed { get => Stats.Get<int>(StatType.Speed); set => Stats.Set(StatType.Speed, value); }
        public int Dexterity { get => Stats.Get<int>(StatType.Dexterity); set => Stats.Set(StatType.Dexterity, value); }
        public int Vitality { get => Stats.Get<int>(StatType.Vitality); set => Stats.Set(StatType.Vitality, value); }
        public int Wisdom { get => Stats.Get<int>(StatType.Wisdom); set => Stats.Set(StatType.Wisdom, value); }

        #endregion

        public Player(User user, DbChar chr) : base((ushort)chr.ClassType)
        {
            User = user;
            Char = chr;

            Name = user.Account.Name;

            Inventory = new PlayerInventory(this);
        }

        public override void Initialize()
        {
            base.Initialize();

            if (World is Realm)
            {
                SendEnemy("Oryx the Mad God", "You are food for my minions!");
                SendInfo("Use [WASDQE] to move; click to shoot!");
            }

            var spawnPos = World.Map.Regions.Where(kvp => kvp.Value == TileRegion.Spawn).RandomElement();
            Move(spawnPos.Key.X, spawnPos.Key.Y);
        }

        protected override void LoadStats()
        {
            if (User.State != ConnectionState.Ready || User.GameInfo.State != GameState.Playing)
                return;

            MaxHP = Char.Stats[0];
            HP = Char.HP;
            MaxMP = Char.Stats[1];
            MP = Char.MP;
            Experience = Char.Experience;
            Level = Char.Level;
            NumStars = User.Account.NumStars;
            Attack = Char.Stats[2];
            Defense = Char.Stats[3];
            Speed = Char.Stats[4];
            Dexterity = Char.Stats[5];
            Vitality = Char.Stats[6];
            Wisdom = Char.Stats[7];
            Inventory.Load(Char);
        }

        public void SaveCharacter()
        {
            Char.Level = Level;
            Char.HP = HP;
            Char.MP = MP;
            Char.Stats[0] = MaxHP;
            Char.Stats[1] = MaxMP;
            Char.Stats[2] = Attack;
            Char.Stats[3] = Defense;
            Char.Stats[4] = Speed;
            Char.Stats[5] = Dexterity;
            Char.Stats[6] = Vitality;
            Char.Stats[7] = Wisdom;
            Inventory.Save(Char);
            DbClient.SaveCharacter(Char);
        }

        public override bool Tick(RealmTime time)
        {
            if (!base.Tick(time))
                return false;

            SendUpdate();
            SendNewTick();

            return true;
        }

        public override void Dispose()
        {
            base.Dispose();

            _visibleTiles.Clear();
            _tilesDiscovered.Clear();
            _newTiles.Clear();
            _deadEntities.Clear();
            _visibleEntities.Clear();
            _newEntities.Clear();
            _oldEntities.Clear();
            _entityStatUpdates.Clear();
            _chunk = null;
        }
    }
}

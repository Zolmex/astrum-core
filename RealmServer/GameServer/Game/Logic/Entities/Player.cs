using Common;
using Common.Database;
using Common.Resources.World;
using Common.Utilities;
using GameServer.Game.Net.Messaging;
using GameServer.Game.Net.Messaging.Outgoing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game.Logic.Entities
{
    public partial class Player : Entity
    {
        public User User { get; }
        public DbChar Char { get; }

        #region STATS

        public int MaxMP { get => Stats.Get<int>(StatType.MaxMP); set => Stats.Set(StatType.MaxMP, value); }
        public int MP { get => Stats.Get<int>(StatType.MP); set => Stats.Set(StatType.MP, value); }
        public int Level { get => Stats.Get<int>(StatType.Level); set => Stats.Set(StatType.Level, value); }
        public int NumStars { get => Stats.Get<int>(StatType.NumStars); set => Stats.Set(StatType.NumStars, value); }

        #endregion

        public Player(User user, DbChar chr)
            : base((ushort)chr.ClassType)
        {
            User = user;
            Char = chr;

            Name = user.Account.Name;
        }

        public override void Initialize()
        {
            base.Initialize();

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
            Level = Char.Level;
            NumStars = User.Account.NumStars;
        }

        public override bool Tick(RealmTime time)
        {
            if (!base.Tick(time))
                return false;

            SendUpdate();
            SendNewTick();

            return true;
        }
    }
}

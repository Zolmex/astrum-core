using Common.Resources.Config;
using Common.Resources.World;
using Common.Utilities;
using GameServer.Game.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game.Worlds
{
    public class Realm : World
    {
        public bool Closed { get; set; }

        public Oryx Oryx { get; }

        public Realm()
            : base(REALM, -1)
        {
            DisplayName = GetRealmName();

            Oryx = new Oryx(this);
        }

        public override void Initialize()
        {
            base.Initialize();

            Oryx.Initialize();
            RealmManager.OnRealmAdded(DisplayName);
        }

        public override void Tick(RealmTime time)
        {
            base.Tick(time);

            Oryx.Tick(time);
        }

        private string GetRealmName()
        {
            string ret = null;
            while (ret == null || RealmManager.ActiveRealms.Contains(ret))
                ret = RealmConfig.Config.Names.RandomElement();
            return ret;
        }
    }
}

using Common.Resources.Config;
using Common.Resources.World;
using Common.Utilities;
using GameServer.Game.Chat;
using GameServer.Game.Logic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game.Logic.Worlds
{
    public class Realm : World
    {
        private readonly string[] _realmNames =
        {
            "Lich", "Goblin", "Ghost",
            "Giant", "Gorgon", "Blob",
            "Leviathan", "Unicorn", "Minotaur",
            "Cube", "Pirate", "Spider",
            "Snake", "Deathmage", "Gargoyle",
            "Scorpion", "Djinn", "Phoenix",
            "Satyr", "Drake", "Orc",
            "Flayer", "Cyclops", "Sprite",
            "Chimera", "Kraken", "Hydra",
            "Slime", "Ogre", "Hobbit",
            "Titan", "Medusa", "Golem",
            "Demon", "Skeleton", "Mummy",
            "Imp", "Bat", "Wyrm",
            "Spectre", "Reaper", "Beholder",
            "Dragon", "Harpy"
        };

        public Realm()
            : base(REALM, -1)
        {
            Name = GetRealmName();
        }

        public override void Initialize()
        {
            base.Initialize();

            RealmManager.OnRealmAdded(Name);
        }

        private string GetRealmName()
        {
            string ret = null;
            while (ret == null || RealmManager.ActiveRealms.Contains(ret))
                ret = _realmNames.RandomElement();
            return ret;
        }
    }
}

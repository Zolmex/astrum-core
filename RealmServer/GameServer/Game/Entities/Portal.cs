using Common.Resources.World;
using Common.Resources.Xml;
using GameServer.Game.Network.Messaging;
using GameServer.Game.Network.Messaging.Outgoing;
using GameServer.Game.Worlds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Game.Entities
{
    public class Portal : Character
    {
        public readonly World PortalWorld;

        public Portal(ushort type) : base(type)
        {
            if (Desc.RealmPortal)
            {
                PortalWorld = new Realm();
                RealmManager.AddWorld(PortalWorld);
                return;
            }

            var worldName = Desc.DungeonName;
            if (worldName == null)
            {
                _log.Error($"Invalid portal type '{type:X4}'");
                return;
            }

            PortalWorld = new World(worldName, -1);
            RealmManager.AddWorld(PortalWorld);
        }

        public override void Initialize()
        {
            base.Initialize();

            Name = PortalWorld.Name + " (" + PortalWorld.Players.Count;

            var maxPlayers = PortalWorld.Config.MaxPlayers;
            if (maxPlayers == -1)
                Name += ")";
            else
                Name += $"/{maxPlayers})";
        }

        public override bool Tick(RealmTime time)
        {
            if (!base.Tick(time))
                return false;

            Name = PortalWorld.Name + " (" + PortalWorld.Players.Count;
            var maxPlayers = PortalWorld.Config.MaxPlayers;
            if (maxPlayers == -1)
                Name += ")";
            else
                Name += $"/{maxPlayers})";
            return true;
        }
    }
}

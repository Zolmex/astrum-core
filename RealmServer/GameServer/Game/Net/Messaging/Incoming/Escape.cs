using Common;
using Common.Utilities.Net;
using Common.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using GameServer.Game.Logic.Entities;
using GameServer.Game.Net.Messaging.Outgoing;
using GameServer.Game.Worlds;

namespace GameServer.Game.Net.Messaging.Incoming
{
    public class Escape : IncomingPacket
    {
        public override PacketId ID => PacketId.ESCAPE;

        protected override void Read(NetworkReader rdr)
        {
        }

        protected override void Handle(User user)
        {
            if (user.GameInfo.State != GameState.Playing)
                return;

            if (user.GameInfo.World.Id == World.NEXUS_ID)
            {
                user.GameInfo.Player.SendInfo("You're already in the Nexus!");
                return;
            }

            user.ReconnectTo(RealmManager.GetWorld(World.NEXUS_ID));
        }

        public override string ToString()
        {
            var type = typeof(Escape);
            var props = type.GetProperties();
            var ret = $"\n";
            foreach (var prop in props)
            {
                ret += $"{prop.Name}:{prop.GetValue(this)}";
                if (!(props.IndexOf(prop) == props.Length - 1))
                    ret += "\n";
            }
            return ret;
        }
    }
}

using Common;
using Common.Utilities.Net;
using Common.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using GameServer.Game.Logic.Entities;

namespace GameServer.Game.Net.Messaging.Incoming
{
    public class UsePortal : IncomingPacket
    {
        public int ObjectId { get; set; }

        public override PacketId ID => PacketId.USEPORTAL;

        protected override void Read(NetworkReader rdr)
        {
            ObjectId = rdr.ReadInt32();
        }

        protected override void Handle(User user)
        {
            if (user.GameInfo.State != GameState.Playing)
                return;

            var entity = user.GameInfo.Player.World.Entities.Get(ObjectId);
            if (entity is not Portal portal)
                return;

            user.ReconnectTo(portal.PortalWorld);
        }

        public override string ToString()
        {
            var type = typeof(UsePortal);
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

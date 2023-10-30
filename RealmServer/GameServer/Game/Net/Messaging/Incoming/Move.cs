using Common;
using Common.Utilities.Net;
using Common.Utilities;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameServer.Game.Net.Messaging.Incoming
{
    public class Move : IncomingPacket
    {
        public WorldPosData Pos { get; set; }

        public override PacketId ID => PacketId.MOVE;

        protected override void Read(NetworkReader rdr)
        {
            Pos = WorldPosData.Read(rdr);
        }

        protected override void Handle(User user)
        {
            if (user.GameInfo.State != GameState.Playing)
                return;

            user.GameInfo.Player.Move(Pos.X, Pos.Y);
        }

        public override string ToString()
        {
            var type = typeof(Move);
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

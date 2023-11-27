using Common.Utilities.Net;
using Common.Utilities;
using System;
using System.IO;
using Common;

namespace GameServer.Game.Network.Messaging.Outgoing
{
    public class Goto : OutgoingPacket
    {
        public WorldPosData Pos { get; set; }

        public override PacketId ID => PacketId.GOTO;

        public static StreamWriteInfo Write(User user, WorldPosData pos)
        {
            var pkt = user.GetPacket(PacketId.GOTO);
            var wtr = pkt.Writer;
            lock (pkt)
            {
                var offset = (int)wtr.BaseStream.Position;
                pos.Write(wtr);
                var length = (int)wtr.BaseStream.Position - offset;
                return new StreamWriteInfo(offset, length);
            }
        }

        public override string ToString()
        {
            var type = typeof(Goto);
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

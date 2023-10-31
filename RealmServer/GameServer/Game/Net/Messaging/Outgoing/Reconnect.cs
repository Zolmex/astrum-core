using Common;
using Common.Utilities.Net;
using Common.Utilities;
using System;
using System.IO;

namespace GameServer.Game.Net.Messaging.Outgoing
{
    public class Reconnect : OutgoingPacket
    {
        public int GameId { get; set; }

        public override PacketId ID => PacketId.RECONNECT;

        public static StreamWriteInfo Write(User user, int gameId)
        {
            var pkt = user.GetPacket(PacketId.RECONNECT);
            var wtr = pkt.Writer;
            lock (pkt)
            {
                var offset = (int)wtr.BaseStream.Position;
                wtr.Write(gameId);
                var length = (int)wtr.BaseStream.Position - offset;
                return new StreamWriteInfo(offset, length);
            }
        }

        public override string ToString()
        {
            var type = typeof(Reconnect);
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

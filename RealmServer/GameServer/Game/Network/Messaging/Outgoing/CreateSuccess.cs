using Common.Utilities.Net;
using Common.Utilities;
using System;
using System.IO;

namespace GameServer.Game.Network.Messaging.Outgoing
{
    public class CreateSuccess : OutgoingPacket
    {
        public int ObjectId { get; set; }
        public int CharId { get; set; }

        public override PacketId ID => PacketId.CREATESUCCESS;

        public static StreamWriteInfo Write(User user, int objectId, int charId)
        {
            var pkt = user.GetPacket(PacketId.CREATESUCCESS);
            var wtr = pkt.Writer;
            lock (pkt)
            {
                var offset = (int)wtr.BaseStream.Position;
                wtr.Write(objectId);
                wtr.Write(charId);
                var length = (int)wtr.BaseStream.Position - offset;
                return new StreamWriteInfo(offset, length);
            }
        }

        public override string ToString()
        {
            var type = typeof(CreateSuccess);
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

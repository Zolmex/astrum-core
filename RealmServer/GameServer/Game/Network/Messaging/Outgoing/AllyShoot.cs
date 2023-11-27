using Common.Utilities.Net;
using Common.Utilities;
using System;
using System.IO;

namespace GameServer.Game.Network.Messaging.Outgoing
{
    public class AllyShoot : OutgoingPacket
    {
        public int OwnerId { get; set; }
        public ushort ContainerType { get; set; }
        public float Angle { get; set; }

        public override PacketId ID => PacketId.ALLYSHOOT;

        public static StreamWriteInfo Write(User user, int ownerId, ushort containerType, float angle)
        {
            var pkt = user.GetPacket(PacketId.ALLYSHOOT);
            var wtr = pkt.Writer;
            lock (pkt)
            {
                var offset = (int)wtr.BaseStream.Position;
                wtr.Write(ownerId);
                wtr.Write(containerType);
                wtr.Write(angle);
                var length = (int)wtr.BaseStream.Position - offset;
                return new StreamWriteInfo(offset, length);
            }
        }

        public override string ToString()
        {
            var type = typeof(AllyShoot);
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

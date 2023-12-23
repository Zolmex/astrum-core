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

        public static void Write(NetworkHandler network, int ownerId, ushort containerType, float angle)
        {
            var state = network.SendState;
            var wtr = state.Writer;
            lock (state)
            {
                var begin = state.PacketBegin();

                wtr.Write(ownerId);
                wtr.Write(containerType);
                wtr.Write(angle);

                state.PacketEnd(begin, PacketId.ALLYSHOOT);
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

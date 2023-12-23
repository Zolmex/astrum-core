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

        public static void Write(NetworkHandler network, int objectId, int charId)
        {
            var state = network.SendState;
            var wtr = state.Writer;
            lock (state)
            {
                var begin = state.PacketBegin();

                wtr.Write(objectId);
                wtr.Write(charId);

                state.PacketEnd(begin, PacketId.CREATESUCCESS);
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

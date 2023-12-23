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

        public static void Write(NetworkHandler network, WorldPosData pos)
        {
            var state = network.SendState;
            var wtr = state.Writer;
            lock (state)
            {
                var begin = state.PacketBegin();

                pos.Write(wtr);

                state.PacketEnd(begin, PacketId.GOTO);
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

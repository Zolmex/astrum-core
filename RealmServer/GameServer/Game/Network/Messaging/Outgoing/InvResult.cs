using Common.Utilities.Net;
using Common.Utilities;
using System;
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GameServer.Game.Network.Messaging.Outgoing
{
    public class InvResult : OutgoingPacket
    {
        public int Result { get; set; }

        public override PacketId ID => PacketId.INVRESULT;

        public static void Write(NetworkHandler network, int result)
        {
            var state = network.SendState;
            var wtr = state.Writer;
            lock (state)
            {
                var begin = state.PacketBegin();

                wtr.Write(result);

                state.PacketEnd(begin, PacketId.INVRESULT);
            }
        }

        public override string ToString()
        {
            var type = typeof(InvResult);
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

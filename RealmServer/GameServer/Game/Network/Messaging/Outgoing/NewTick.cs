using Common;
using Common.Utilities.Net;
using Common.Utilities;
using System;
using System.IO;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GameServer.Game.Network.Messaging.Outgoing
{
    public class NewTick : OutgoingPacket
    {
        public ObjectStatusData[] Statuses { get; set; }

        public override PacketId ID => PacketId.NEWTICK;

        public static void Write(NetworkHandler network, Dictionary<int, ObjectStatusData> statuses)
        {
            var state = network.SendState;
            var wtr = state.Writer;
            lock (state)
            {
                var begin = state.PacketBegin();

                wtr.Write((short)statuses.Count);
                foreach (var kvp in statuses)
                    kvp.Value.Write(wtr);

                state.PacketEnd(begin, PacketId.NEWTICK);
            }
        }

        public override string ToString()
        {
            var type = typeof(NewTick);
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

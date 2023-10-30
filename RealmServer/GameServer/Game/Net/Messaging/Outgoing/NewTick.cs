using Common;
using Common.Utilities.Net;
using Common.Utilities;
using System;
using System.IO;
using System.Collections.Generic;

namespace GameServer.Game.Net.Messaging.Outgoing
{
    public class NewTick : OutgoingPacket
    {
        public ObjectStatusData[] Statuses { get; set; }

        public override PacketId ID => PacketId.NEWTICK;

        public static StreamWriteInfo Write(User user, Dictionary<int, ObjectStatusData> statuses)
        {
            var pkt = user.GetPacket(PacketId.NEWTICK);
            var wtr = pkt.Writer;
            lock (pkt)
            {
                var offset = (int)wtr.BaseStream.Position;
                wtr.Write((short)statuses.Count);
                foreach (var kvp in statuses)
                    kvp.Value.Write(wtr);
                var length = (int)wtr.BaseStream.Position - offset;
                return new StreamWriteInfo(offset, length);
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

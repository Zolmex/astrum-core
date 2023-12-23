using Common;
using Common.Utilities.Net;
using Common.Utilities;
using System;
using System.IO;

namespace GameServer.Game.Network.Messaging.Outgoing
{
    public class Reconnect : OutgoingPacket
    {
        public int GameId { get; set; }

        public override PacketId ID => PacketId.RECONNECT;

        public static void Write(NetworkHandler network, int gameId)
        {
            var state = network.SendState;
            var wtr = state.Writer;
            lock (state)
            {
                var begin = state.PacketBegin();

                wtr.Write(gameId);

                state.PacketEnd(begin, PacketId.RECONNECT);
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

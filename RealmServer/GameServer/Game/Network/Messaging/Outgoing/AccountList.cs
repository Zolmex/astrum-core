using Common.Utilities.Net;
using Common.Utilities;
using System;
using System.IO;
using System.Net.Sockets;

namespace GameServer.Game.Network.Messaging.Outgoing
{
    public class AccountList : OutgoingPacket
    {
        public const int Locked = 0;
        public const int Ignored = 1;

        public int AccountListId { get; set; }
        public int[] AccountIds { get; set; }

        public override PacketId ID => PacketId.ACCOUNTLIST;

        public static void Write(NetworkHandler network, int accListId, int[] accIds)
        {
            var state = network.SendState;
            var wtr = state.Writer;
            lock (state)
            {
                var begin = state.PacketBegin();

                wtr.Write(accListId);
                wtr.Write((short)accIds.Length);
                foreach (var id in accIds)
                    wtr.Write(id);

                state.PacketEnd(begin, PacketId.ACCOUNTLIST);
            }
        }

        public override string ToString()
        {
            var type = typeof(AccountList);
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

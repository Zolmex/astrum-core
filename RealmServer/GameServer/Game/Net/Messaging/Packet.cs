using Common.Utilities;
using Common.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;

namespace GameServer.Game.Net.Messaging
{
    public abstract class Packet
    {
        protected readonly Logger _log = new Logger(typeof(Packet));

        public NetworkHandler Handler { get; private set; }
        public abstract PacketId ID { get; }

        public static Dictionary<PacketId, IncomingPacket> LoadIncoming(NetworkHandler handler)
        {
            var ret = new Dictionary<PacketId, IncomingPacket>();
            var types = Assembly.GetExecutingAssembly().GetTypes();
            for (var i = 0; i < types.Length; i++)
            {
                var type = types[i];
                if (!type.IsAbstract && type.IsSubclassOf(typeof(IncomingPacket)))
                {
                    var pkt = (IncomingPacket)Activator.CreateInstance(type);
                    pkt.Handler = handler;

                    ret.Add(pkt.ID, pkt);
                }
            }
            return ret;
        }

        public static Dictionary<PacketId, OutgoingPacket> LoadOutgoing(NetworkHandler handler) // Yes, load ALL packets for EVERY client
        {
            var ret = new Dictionary<PacketId, OutgoingPacket>();
            var types = Assembly.GetExecutingAssembly().GetTypes();
            for (var i = 0; i < types.Length; i++)
            {
                var type = types[i];
                if (!type.IsAbstract && type.IsSubclassOf(typeof(OutgoingPacket)))
                {
                    var pkt = (OutgoingPacket)Activator.CreateInstance(type);
                    pkt.Handler = handler;

                    ret.Add(pkt.ID, pkt);
                }
            }
            return ret;
        }
    }
}

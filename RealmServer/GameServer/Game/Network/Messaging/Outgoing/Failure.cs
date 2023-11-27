using Common;
using Common.Utilities.Net;
using Common.Utilities;
using System;
using System.IO;

namespace GameServer.Game.Network.Messaging.Outgoing
{
    public class Failure : OutgoingPacket
    {
        public const string DEFAULT_MESSAGE = "An error occured while processing data from your client.";
        public const int DEFAULT = 0;
        public const int INCORRECT_VERSION = 1;
        public const int FORCE_CLOSE_GAME = 2;
        public const int INVALID_TELEPORT_TARGET = 3;
        public const int ACCOUNT_IN_USE = 4;

        public int ErrorId { get; set; }
        public string ErrorDescription { get; set; }

        public override PacketId ID => PacketId.FAILURE;

        public static StreamWriteInfo Write(User user, int errorId, string errorDescription)
        {
            var pkt = user.GetPacket(PacketId.FAILURE);
            var wtr = pkt.Writer;
            lock (pkt)
            {
                var offset = (int)wtr.BaseStream.Position;
                wtr.Write(errorId);
                wtr.WriteUTF(errorDescription);
                var length = (int)wtr.BaseStream.Position - offset;
                return new StreamWriteInfo(offset, length);
            }
        }

        public override string ToString()
        {
            var type = typeof(Failure);
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

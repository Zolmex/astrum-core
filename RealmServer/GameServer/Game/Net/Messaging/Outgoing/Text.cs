using Common;
using Common.Utilities.Net;
using Common.Utilities;
using System;
using System.IO;

namespace GameServer.Game.Net.Messaging.Outgoing
{
    public class Text : OutgoingPacket
    {
        public string Name { get; set; }
        public int ObjectId { get; set; }
        public int NumStars { get; set; }
        public byte BubbleTime { get; set; }
        public string Recipent { get; set; }
        public string Txt { get; set; }

        public override PacketId ID => PacketId.TEXT;

        public static StreamWriteInfo Write(User user, string name, int objId, int numStars, byte bubbleTime, string recipent, string text)
        {
            var pkt = user.GetPacket(PacketId.TEXT);
            var wtr = pkt.Writer;
            lock (pkt)
            {
                var offset = (int)wtr.BaseStream.Position;
                wtr.WriteUTF(name);
                wtr.Write(objId);
                wtr.Write(numStars);
                wtr.Write(bubbleTime);
                wtr.WriteUTF(recipent);
                wtr.WriteUTF(text);
                var length = (int)wtr.BaseStream.Position - offset;
                return new StreamWriteInfo(offset, length);
            }
        }

        public override string ToString()
        {
            var type = typeof(Text);
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

using Common;
using Common.Utilities.Net;
using Common.Utilities;
using System;
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GameServer.Game.Network.Messaging.Outgoing
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

        public static void Write(NetworkHandler network, string name, int objId, int numStars, byte bubbleTime, string recipent, string text)
        {
            var state = network.SendState;
            var wtr = state.Writer;
            lock (state)
            {
                var begin = state.PacketBegin();

                wtr.WriteUTF(name);
                wtr.Write(objId);
                wtr.Write(numStars);
                wtr.Write(bubbleTime);
                wtr.WriteUTF(recipent);
                wtr.WriteUTF(text);

                state.PacketEnd(begin, PacketId.TEXT);
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

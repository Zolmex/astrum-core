using Common.Utilities.Net;
using Common.Utilities;
using System;
using System.IO;

namespace GameServer.Game.Net.Messaging.Outgoing
{
    public class MapInfo : OutgoingPacket
    {
        public int MapWidth { get; set; }
        public int MapHeight { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public uint Seed { get; set; }
        public int Background { get; set; }
        public bool AllowPlayerTeleport { get; set; }
        public bool ShowDisplays { get; set; }

        public override PacketId ID => PacketId.MAPINFO;

        public static StreamWriteInfo Write(User user, int mapWidth, int mapHeight, string name, string displayName, uint seed, int background, bool allowPlayerTeleport, bool showDisplays)
        {
            var pkt = user.GetPacket(PacketId.MAPINFO);
            var wtr = pkt.Writer;
            lock (pkt)
            {
                var offset = (int)wtr.BaseStream.Position;
                wtr.Write(mapWidth);
                wtr.Write(mapHeight);
                wtr.WriteUTF(name);
                wtr.WriteUTF(displayName);
                wtr.Write(seed);
                wtr.Write(background);
                wtr.Write(allowPlayerTeleport);
                wtr.Write(showDisplays);
                var length = (int)wtr.BaseStream.Position - offset;
                return new StreamWriteInfo(offset, length);
            }
        }

        public override string ToString()
        {
            var type = typeof(MapInfo);
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

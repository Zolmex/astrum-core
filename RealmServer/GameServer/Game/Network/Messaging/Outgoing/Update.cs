using Common;
using Common.Utilities.Net;
using Common.Utilities;
using System;
using System.IO;
using System.Collections.Generic;
using GameServer.Game.Worlds;

namespace GameServer.Game.Network.Messaging.Outgoing
{
    public class Update : OutgoingPacket
    {
        public WorldTile[] NewTiles { get; set; }
        public ObjectData[] NewEntities { get; set; }
        public ObjectDropData[] OldEntities { get; set; }

        public override PacketId ID => PacketId.UPDATE;

        public static StreamWriteInfo Write(User user, List<WorldTile> tiles, List<ObjectData> newEntities, List<ObjectDropData> oldEntities)
        {
            var pkt = user.GetPacket(PacketId.UPDATE);
            var wtr = pkt.Writer;
            lock (pkt)
            {
                var offset = (int)wtr.BaseStream.Position;
                wtr.Write((short)tiles.Count);
                for (var i = 0; i < tiles.Count; i++)
                    tiles[i].Write(wtr);
                wtr.Write((short)newEntities.Count);
                for (var i = 0; i < newEntities.Count; i++)
                    newEntities[i].Write(wtr);
                wtr.Write((short)oldEntities.Count);
                for (var i = 0; i < oldEntities.Count; i++)
                    oldEntities[i].Write(wtr);
                var length = (int)wtr.BaseStream.Position - offset;
                return new StreamWriteInfo(offset, length);
            }
        }

        public override string ToString()
        {
            var type = typeof(Update);
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

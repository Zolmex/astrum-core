using Common;
using Common.Utilities.Net;
using Common.Utilities;
using System;
using System.IO;

namespace GameServer.Game.Net.Messaging.Outgoing
{
    public class EnemyShoot : OutgoingPacket
    {
        public byte BulletId { get; set; }
        public int OwnerId { get; set; }
        public byte BulletType { get; set; }
        public WorldPosData StartPosition { get; set; }
        public float Angle { get; set; }
        public short Damage { get; set; }
        public byte NumShots { get; set; }
        public float AngleInc { get; set; }

        public override PacketId ID => PacketId.ENEMYSHOOT;

        public static StreamWriteInfo Write(User user, byte bulletId, int ownerId, byte bulletType, WorldPosData startPos, float angle, short dmg, byte numShots, float angleInc)
        {
            var pkt = user.GetPacket(PacketId.ENEMYSHOOT);
            var wtr = pkt.Writer;
            lock (pkt)
            {
                var offset = (int)wtr.BaseStream.Position;
                wtr.Write(bulletId);
                wtr.Write(ownerId);
                wtr.Write(bulletType);
                startPos.Write(wtr);
                wtr.Write(angle);
                wtr.Write(dmg);
                if (numShots > 1)
                {
                    wtr.Write(numShots);
                    wtr.Write(angleInc);
                }
                var length = (int)wtr.BaseStream.Position - offset;
                return new StreamWriteInfo(offset, length);
            }
        }

        public override string ToString()
        {
            var type = typeof(EnemyShoot);
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

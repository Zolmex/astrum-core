using Common;
using Common.Utilities.Net;
using Common.Utilities;
using System;
using System.IO;

namespace GameServer.Game.Network.Messaging.Outgoing
{
    public class EnemyShoot : OutgoingPacket
    {
        public int OwnerId { get; set; }
        public int BulletId { get; set; }
        public byte BulletType { get; set; }
        public WorldPosData StartPosition { get; set; }
        public float Angle { get; set; }
        public uint Damage { get; set; }
        public byte NumShots { get; set; }
        public float AngleInc { get; set; }

        public override PacketId ID => PacketId.ENEMYSHOOT;

        public static void Write(NetworkHandler network, int bulletId, int ownerId, byte bulletType, WorldPosData startPos, float angle, uint dmg, byte numShots, float angleInc)
        {
            var state = network.SendState;
            var wtr = state.Writer;
            lock (state)
            {
                var begin = state.PacketBegin();

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

                state.PacketEnd(begin, PacketId.ENEMYSHOOT);
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

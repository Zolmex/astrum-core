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
        public int ProjId { get; set; }
        public byte BulletId { get; set; }
        public WorldPosData StartPosition { get; set; }
        public float Angle { get; set; }
        public uint Damage { get; set; }

        public override PacketId ID => PacketId.ENEMYSHOOT;

        public static void Write(NetworkHandler network, byte projId, int ownerId, byte bulletId, WorldPosData startPos, float angle, uint dmg)
        {
            var state = network.SendState;
            var wtr = state.Writer;
            lock (state)
            {
                var begin = state.PacketBegin();

                wtr.Write(projId);
                wtr.Write(ownerId);
                wtr.Write(bulletId);
                startPos.Write(wtr);
                wtr.Write(angle);
                wtr.Write(dmg);

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

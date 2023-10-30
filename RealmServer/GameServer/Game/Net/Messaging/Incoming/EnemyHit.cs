using Common.Utilities.Net;
using Common.Utilities;
using System;

namespace GameServer.Game.Net.Messaging.Incoming
{
    public class EnemyHit : IncomingPacket
    {
        public int BulletId { get; set; }
        public int TargetId { get; set; }

        public override PacketId ID => PacketId.ENEMYHIT;

        protected override void Read(NetworkReader rdr)
        {
            BulletId = rdr.ReadByte();
            TargetId = rdr.ReadInt32();
        }

        protected override void Handle(User user)
        {
            // TODO
        }

        public override string ToString()
        {
            var type = typeof(EnemyHit);
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

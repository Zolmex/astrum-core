using Common.Utilities.Net;
using Common.Utilities;
using System;

namespace GameServer.Game.Net.Messaging.Incoming
{
    public class PlayerHit : IncomingPacket
    {
        public int OwnerId { get; set; }
        public byte ProjectileId { get; set; }

        public override PacketId ID => PacketId.PLAYERHIT;

        protected override void Read(NetworkReader rdr)
        {
            OwnerId = rdr.ReadInt32();
            ProjectileId = rdr.ReadByte();
        }

        protected override void Handle(User user)
        {
            // TODO
        }

        public override string ToString()
        {
            var type = typeof(PlayerHit);
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
